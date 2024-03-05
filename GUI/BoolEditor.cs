using StructureHelper.Helpers;
using System;
using Terraria.UI;

namespace StructureHelper.GUI
{
	internal class BoolEditor : FieldEditor<bool>
	{
		public BoolEditor(string name, Action<bool> onValueChanged, bool initialValue, Func<bool> listenForUpdate = null, string description = "") : base(70, name, onValueChanged, listenForUpdate, initialValue, description) { }

		public override void SafeClick(UIMouseEvent evt)
		{
			value = !value;
			onValueChanged(value);

			Main.isMouseLeftConsumedByUI = true;
		}

		public override void SafeDraw(SpriteBatch sprite)
		{
			Utils.DrawBorderString(sprite, $"{value}", GetDimensions().Position() + new Vector2(12, 38), Color.White, 0.8f);

			var box = GetDimensions().ToRectangle();
			box.Width = 40;
			box.Height = 15;
			box.Offset(new Point(95, 40));
			GUIHelper.DrawBox(sprite, box);

			if (value)
			{
				box.Width = 15;
				box.Offset(new Point(25, 0));
				GUIHelper.DrawBox(sprite, box, Color.Yellow);
			}
			else
			{
				box.Width = 15;
				GUIHelper.DrawBox(sprite, box);
			}
		}
	}
}