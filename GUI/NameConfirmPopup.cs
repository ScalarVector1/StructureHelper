using StructureHelper.Core.Loaders.UILoading;
using System;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StructureHelper.GUI
{
	internal class NameConfirmPopup : SmartUIState
	{
		public static bool visible;
		public static Action<string> onConfirm;

		public TextField nameField = new();
		public UIText confirm = new("Confirm");
		public UIText cancel = new("Cancel");

		public override bool Visible => visible;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public static void OpenConfirmation(Action<string> onConfirm)
		{
			visible = true;
			NameConfirmPopup.onConfirm = onConfirm;
		}

		public override void OnInitialize()
		{
			confirm.OnMouseOver += (a, b) => confirm.TextColor = Color.Yellow;
			confirm.OnMouseOut += (a, b) => confirm.TextColor = Color.White;
			confirm.PaddingTop = 6;
			confirm.OnLeftClick += (a, b) =>
			{
				if (visible)
				{
					onConfirm?.Invoke(nameField.currentValue);
					nameField.currentValue = "";
					visible = false;
				}
			};

			cancel.OnMouseOver += (a, b) => cancel.TextColor = Color.Yellow;
			cancel.OnMouseOut += (a, b) => cancel.TextColor = Color.White;
			cancel.PaddingTop = 6;
			cancel.OnLeftClick += (a, b) => visible = false;

			AddElement(nameField, -120, 0.5f, -46, 0.5f, 240, 0, 28, 0);
			AddElement(confirm, -100, 0.5f, 0, 0.5f, 80, 0, 32, 0);
			AddElement(cancel, 20, 0.5f, 0, 0.5f, 80, 0, 32, 0);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var color = new Color(49, 84, 141);
			var back = new Rectangle(Main.screenWidth / 2 - 150, Main.screenHeight / 2 - 100, 300, 150);

			if (back.Contains(Main.MouseScreen.ToPoint()))
				Main.LocalPlayer.mouseInterface = true;

			Helpers.GUIHelper.DrawBox(spriteBatch, back, color);
			Utils.DrawBorderString(spriteBatch, "Name your creation:", new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - 70), Color.White, 1, 0.5f, 0.5f);

			Helpers.GUIHelper.DrawBox(spriteBatch, confirm.GetDimensions().ToRectangle(), color);
			Helpers.GUIHelper.DrawBox(spriteBatch, cancel.GetDimensions().ToRectangle(), color);

			base.Draw(spriteBatch);
			Recalculate();
		}
	}
}