using StructureHelper.ChestHelper;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StructureHelper.Content.GUI.ChestGUI
{
	class LootElement : UIElement
	{
		readonly Loot loot;
		readonly UIImageButton removeButton = new(ModContent.Request<Texture2D>("StructureHelper/GUI/Cross"));

		readonly NumberSetter min;
		readonly NumberSetter max;
		readonly NumberSetter weight;

		readonly UIImageButton upButton = new(ModContent.Request<Texture2D>("StructureHelper/GUI/Up"));
		readonly UIImageButton downButton = new(ModContent.Request<Texture2D>("StructureHelper/GUI/Down"));

		public LootElement(Loot loot, bool hasWeight)
		{
			this.loot = loot;

			Width.Set(350, 0);
			Height.Set(50, 0);
			Left.Set(50, 0);

			removeButton.Left.Set(-36, 1);
			removeButton.Top.Set(6, 0);
			removeButton.Width.Set(32, 0);
			removeButton.Height.Set(32, 0);
			removeButton.OnLeftClick += Remove;
			Append(removeButton);

			min = new NumberSetter(loot.min, "Min", 115);
			Append(min);

			max = new NumberSetter(loot.max, "Max", 70);
			Append(max);

			if (hasWeight)
			{
				weight = new NumberSetter(loot.weight, "Weight", 160);
				Append(weight);
			}
			else
			{
				upButton.Left.Set(8, 0);
				upButton.Top.Set(10, 0);
				upButton.Width.Set(12, 0);
				upButton.Height.Set(8, 0);
				upButton.SetVisibility(1, 0.8f);
				upButton.OnLeftClick += MoveUp;
				Append(upButton);

				downButton.Left.Set(8, 0);
				downButton.Top.Set(26, 0);
				downButton.Width.Set(12, 0);
				downButton.Height.Set(8, 0);
				downButton.SetVisibility(1, 0.8f);
				downButton.OnLeftClick += MoveDown;
				Append(downButton);
			}
		}

		private void MoveDown(UIMouseEvent evt, UIElement listeningElement)
		{
			var list = Parent.Parent as UIList;
			int i = list._items.IndexOf(this);

			if (i < list.Count - 1)
			{
				(list._items[i + 1], list._items[i]) = (list._items[i], list._items[i + 1]);
			}
		}

		private void MoveUp(UIMouseEvent evt, UIElement listeningElement)
		{
			var list = Parent.Parent as UIList;
			int i = list._items.IndexOf(this);

			if (i >= 1)
			{
				(list._items[i - 1], list._items[i]) = (list._items[i], list._items[i - 1]);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 pos = GetDimensions().ToRectangle().TopLeft();
			var target = new Rectangle((int)pos.X, (int)pos.Y, (int)GetDimensions().Width, 46);

			Color color = Color.White;

			if (Parent.Parent.Parent is ChestRuleElement)
			{
				color = (Parent.Parent.Parent as ChestRuleElement).color;
				color = Color.Lerp(color, Color.Black, 0.25f);
			}

			if (removeButton.IsMouseHovering)
			{
				Tooltip.SetName("Remove item");
				Tooltip.SetTooltip("Remove this item from the rule");
			}

			Helpers.GUIHelper.DrawBox(spriteBatch, target, color);

			int xOff = 0;
			if (weight is null)
				xOff += 20;

			Main.inventoryScale = 36 / 52f * 46 / 36f;
			ItemSlot.Draw(spriteBatch, ref loot.givenItem, 21, pos + Vector2.UnitX * xOff);

			string name = loot.givenItem.Name.Length > 20 ? loot.givenItem.Name[..18] + "..." : loot.givenItem.Name;
			Utils.DrawBorderString(spriteBatch, name, pos + new Vector2(46 + xOff, 14), Color.White, 0.8f);

			if (max.Value < min.Value)
				max.Value = min.Value;

			loot.min = min.Value;
			loot.max = max.Value;

			if (weight != null)
				loot.weight = weight.Value;

			base.Draw(spriteBatch);
		}

		private void Remove(UIMouseEvent evt, UIElement listeningElement)
		{
			if (!(Parent.Parent.Parent is ChestRuleElement))
				return;

			var parent = Parent.Parent.Parent as ChestRuleElement;
			parent.RemoveItem(loot, this);
		}
	}
}