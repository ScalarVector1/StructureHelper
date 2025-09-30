using StructureHelper.API;
using StructureHelper.Content.Items;
using StructureHelper.Core;
using StructureHelper.Core.Loaders.UILoading;
using StructureHelper.Models;
using StructureHelper.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace StructureHelper.Content.GUI
{
	internal class TileDataConfiguratorMenu : SmartUIState
	{
		public static UIList toggles = new();
		public static UIImageButton closeButton = new(Assets.GUI.Cross);

		public static UIScrollbar scrollBar = new();

		public static bool visible;
		public override bool Visible => visible;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public override void OnInitialize()
		{
			SetDims(toggles, -200, 0.5f, 0, 0.1f, 400, 0, 0, 0.8f);
			SetDims(scrollBar, 232, 0.5f, 0, 0.1f, 32, 0, 0, 0.8f);
			toggles.SetScrollbar(scrollBar);
			Append(toggles);
			Append(scrollBar);

			SetDims(closeButton, 200 - 32, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			closeButton.OnLeftClick += CloseButton_OnClick;
			Append(closeButton);
		}

		private void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			visible = false;
			Main.isMouseLeftConsumedByUI = true;
		}

		public static void OpenMenu()
		{
			visible = true;

			toggles.Clear();

			foreach(var pair in WandSavingSettings.possibleCustomDataTypes)
			{
				toggles.Add(new CustomDataToggleButton(pair.Value, pair.Key));
			}
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			Recalculate();

			if (closeButton.IsMouseHovering)
			{
				Tooltip.SetName("Close");
				Tooltip.SetTooltip("Close this menu");
				Main.LocalPlayer.mouseInterface = true;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var color = new Color(49, 84, 141);
			Helpers.GUIHelper.DrawBox(spriteBatch, closeButton.GetDimensions().ToRectangle(), closeButton.IsMouseHovering ? color : color * 0.8f);

			var rect = toggles.GetDimensions().ToRectangle();
			rect.Inflate(30, 10);
			Helpers.GUIHelper.DrawBox(spriteBatch, rect, new Color(20, 40, 60) * 0.8f);

			base.Draw(spriteBatch);
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

	class CustomDataToggleButton : UIElement
	{
		public Type typeToToggle;
		public string key;

		bool Active => WandSavingSettings.activeCustomDataTypes.Contains(typeToToggle);

		public CustomDataToggleButton(Type typeToToggle, string key)
		{
			this.typeToToggle = typeToToggle;
			this.key = key;

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
			Utils.DrawBorderString(spriteBatch, key, mainBox.Center() + Vector2.UnitY * 4, color, 0.8f, 0.5f, 0.5f);

			base.Draw(spriteBatch);
		}

		public override void LeftClick(UIMouseEvent evt)
		{
			if (Active)
				WandSavingSettings.DeactivateTypeForSaving(key);
			else
				WandSavingSettings.ActivateTypeForSaving(key);
		}

		public override int CompareTo(object obj)
		{
			if (obj is CustomDataToggleButton other)
				return key.CompareTo(other.key);

			return base.CompareTo(obj);
		}
	}
}
