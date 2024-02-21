using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StructureHelper.Core.Loaders.UILoading;
using StructureHelper.Items;
using StructureHelper.Util;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace StructureHelper.GUI
{
	class ManualGeneratorMenu : SmartUIState
	{
		public static StructureEntry selected;
		public static bool ignoreNulls = false;

		public static StructurePreview preview;

		public static bool multiMode = false;
		public static int multiIndex;

		public static UIList structureElements = new();
		public static UIScrollbar scrollBar = new();

		public static UIImageButton refreshButton = new(ModContent.Request<Texture2D>("StructureHelper/GUI/Refresh"));
		public static UIImageButton ignoreButton = new(ModContent.Request<Texture2D>("StructureHelper/GUI/Null"));
		public static UIImageButton closeButton = new(ModContent.Request<Texture2D>("StructureHelper/GUI/Cross"));

		public override bool Visible => TestWand.UIVisible;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		/// <summary>
		/// Loads all structure files and generates elements to be clicked for them
		/// </summary>
		public static void LoadStructures()
		{
			structureElements.Clear();
			selected = null;

			string folderPath = ModLoader.ModPath.Replace("Mods", "SavedStructures");
			Directory.CreateDirectory(folderPath);

			string[] filePaths = Directory.GetFiles(folderPath);

			foreach (string path in filePaths)
			{
				string name = path.Replace(folderPath + Path.DirectorySeparatorChar, "");
				structureElements.Add(new StructureEntry(name, path));
			}
		}

		public override void OnInitialize()
		{
			LoadStructures();
			SetDims(structureElements, -200, 0.5f, 0, 0.1f, 400, 0, 0, 0.8f);
			SetDims(scrollBar, 232, 0.5f, 0, 0.1f, 32, 0, 0, 0.8f);
			structureElements.SetScrollbar(scrollBar);
			Append(structureElements);
			Append(scrollBar);

			SetDims(refreshButton, -200, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			refreshButton.OnLeftClick += RefreshButton_OnClick;
			Append(refreshButton);

			SetDims(ignoreButton, -150, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			ignoreButton.OnLeftClick += IgnoreButton_OnClick;
			Append(ignoreButton);

			SetDims(closeButton, 200 - 32, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			closeButton.OnLeftClick += CloseButton_OnClick;
			Append(closeButton);
		}

		private void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			TestWand.UIVisible = false;
			Main.isMouseLeftConsumedByUI = true;
		}

		private void IgnoreButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			ignoreNulls = !ignoreNulls;
			Main.isMouseLeftConsumedByUI = true;
		}

		private void RefreshButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			LoadStructures();
			Main.isMouseLeftConsumedByUI = true;
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			Recalculate();

			if (Main.playerInventory)
				TestWand.UIVisible = false;

			if (ignoreButton.IsMouseHovering)
			{
				Tooltip.SetName($"Place with null tiles: {ignoreNulls}");
				Tooltip.SetTooltip("If the structure placed manually should have it's null tiles placed or not. Turn this off to get a realistic generation, or on if you want to edit the structure.");
			}

			if (refreshButton.IsMouseHovering)
			{
				Tooltip.SetName("Reload");
				Tooltip.SetTooltip("Reload structures from the folder, use this if you change the folders contents externally and want to see it reflected here.");
			}

			if (closeButton.IsMouseHovering)
			{
				Tooltip.SetName("Close");
				Tooltip.SetTooltip("Close this menu");
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var color = new Color(49, 84, 141);
			Helpers.GUIHelper.DrawBox(spriteBatch, ignoreButton.GetDimensions().ToRectangle(), ignoreButton.IsMouseHovering ? color : color * 0.8f);
			Helpers.GUIHelper.DrawBox(spriteBatch, refreshButton.GetDimensions().ToRectangle(), refreshButton.IsMouseHovering ? color : color * 0.8f);
			Helpers.GUIHelper.DrawBox(spriteBatch, closeButton.GetDimensions().ToRectangle(), closeButton.IsMouseHovering ? color : color * 0.8f);

			var rect = structureElements.GetDimensions().ToRectangle();
			rect.Inflate(30, 10);
			Helpers.GUIHelper.DrawBox(spriteBatch, rect, new Color(20, 40, 60) * 0.8f);

			base.Draw(spriteBatch);

			if (!ignoreNulls)
			{
				Texture2D tex = ModContent.Request<Texture2D>("StructureHelper/GUI/Cross").Value;
				spriteBatch.Draw(tex, ignoreButton.GetDimensions().ToRectangle(), ignoreButton.IsMouseHovering ? Color.White : Color.White * 0.5f);
			}
		}

		public static void SetDims(UIElement ele, int x, int y, int w, int h)
		{
			ele.Left.Set(x, 0);
			ele.Top.Set(y, 0);
			ele.Width.Set(w, 0);
			ele.Height.Set(h, 0);
		}

		public static void SetDims(UIElement ele, int x, float xp, int y, float yp, int w, float wp, int h, float hp)
		{
			ele.Left.Set(x, xp);
			ele.Top.Set(y, yp);
			ele.Width.Set(w, wp);
			ele.Height.Set(h, hp);
		}
	}

	class StructureEntry : UIElement
	{
		public string name = "";
		public string path;

		bool Active => ManualGeneratorMenu.selected == this;

		public StructureEntry(string name, string path)
		{
			this.name = name;
			this.path = path;

			Width.Set(400, 0);
			Height.Set(32, 0);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;

			Vector2 pos = GetDimensions().ToRectangle().TopLeft();
			var mainBox = new Rectangle((int)pos.X, (int)pos.Y, 400, 32);

			Color color = Color.Gray;

			if (IsMouseHovering)
				color = Color.White;

			if (Active)
				color = Color.Yellow;

			Helpers.GUIHelper.DrawBox(spriteBatch, mainBox, IsMouseHovering || Active ? new Color(49, 84, 141) : new Color(49, 84, 141) * 0.6f);
			Utils.DrawBorderString(spriteBatch, name, mainBox.Center() + Vector2.UnitY * 4, color, 0.8f, 0.5f, 0.5f);

			base.Draw(spriteBatch);

			if (!Active)
			{
				Height.Set(32, 0);
				RemoveAllChildren();
			}
		}

		public override void LeftClick(UIMouseEvent evt)
		{
			ManualGeneratorMenu.selected = this;
			ManualGeneratorMenu.multiIndex = 0;

			if (!Generator.StructureDataCache.ContainsKey(path))
			{
				Generator.LoadFile(path, StructureHelper.Instance, true);
			}

			if (Generator.StructureDataCache[path].ContainsKey("Structures"))
			{
				ManualGeneratorMenu.multiMode = true;

				int count = Generator.StructureDataCache[path].Get<List<TagCompound>>("Structures").Count;
				Height.Set(36 + 36 * count, 0);

				var list = new UIList();

				for (int k = 0; k < count; k++)
				{
					list.Add(new MultiSelectionEntry(k));
				}

				list.Width.Set(300, 0);
				list.Height.Set(36 * count, 0);
				list.Left.Set(50, 0);
				list.Top.Set(36, 0);
				Append(list);
			}
			else
			{
				ManualGeneratorMenu.multiMode = false;

				ManualGeneratorMenu.preview?.Dispose();
				ManualGeneratorMenu.preview = new StructurePreview(name, Generator.StructureDataCache[path]);
			}
		}
	}

	class MultiSelectionEntry : UIElement
	{
		public int value;

		bool Active => ManualGeneratorMenu.multiIndex == value;

		public MultiSelectionEntry(int index)
		{
			value = index;
			Width.Set(50, 0);
			Height.Set(32, 0);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;

			Vector2 pos = GetDimensions().ToRectangle().Center();
			Color color = Color.Gray;

			if (IsMouseHovering)
				color = Color.White;

			if (Active)
				color = Color.Yellow;

			Helpers.GUIHelper.DrawBox(spriteBatch, GetDimensions().ToRectangle(), IsMouseHovering || Active ? new Color(49, 84, 141) : new Color(49, 84, 141) * 0.6f);
			Utils.DrawBorderString(spriteBatch, value.ToString(), pos + Vector2.UnitY * 4, color, 0.8f, 0.5f, 0.5f);

			base.Draw(spriteBatch);
		}

		public override void LeftClick(UIMouseEvent evt)
		{
			ManualGeneratorMenu.multiIndex = value;
		}
	}
}
