using StructureHelper.GUI;
using Terraria.UI;

namespace StructureHelper.ChestHelper.GUI
{
	class NumberSetter : UIElement
	{
		readonly string Text;
		readonly string Suffix;

		readonly TextField editor = new(InputType.integer);

		public int Value
		{
			get
			{
				if (int.TryParse(editor.currentValue, out int result))
					return result;

				return 0;
			}

			set => editor.currentValue = value.ToString();
		}

		public NumberSetter(int value, string text, int xOff, string suffix = "")
		{
			Value = value;
			Text = text;
			Suffix = suffix;

			Width.Set(32, 0);
			Height.Set(50, 0);
			Left.Set(-xOff, 1);

			editor.Left.Set(0, 0);
			editor.Top.Set(16, 0);
			editor.Width.Set(32, 0);
			editor.Height.Set(22, 0);
			Append(editor);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsMouseHovering)
			{
				Tooltip.SetName(Text);
				Tooltip.SetTooltip("Click to type");
			}

			base.Draw(spriteBatch);

			Utils.DrawBorderString(spriteBatch, Text, GetDimensions().Center() + Vector2.UnitY * -22, Color.White, 0.7f, 0.5f, 0f);
			Utils.DrawBorderString(spriteBatch, Suffix, GetDimensions().Center() + new Vector2(16, 6), Color.White, 0.7f, 0.5f, 0.5f);
		}
	}
}