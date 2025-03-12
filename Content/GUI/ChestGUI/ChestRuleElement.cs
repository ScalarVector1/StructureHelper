using StructureHelper.ChestHelper;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StructureHelper.Content.GUI.ChestGUI
{
	class ChestRuleElement : UIElement
	{
		internal ChestRule rule;
		internal Color color = Color.White;
		internal float storedHeight = 0;

		internal UIList lootElements = [];
		readonly UIImageButton removeButton = new(Assets.GUI.Cross);

		readonly UIImageButton upButton = new(Assets.GUI.UpLarge);
		readonly UIImageButton downButton = new(Assets.GUI.DownLarge);
		readonly UIImageButton hideButton = new(Assets.GUI.Eye);

		public ChestRuleElement(ChestRule rule)
		{
			this.rule = rule;

			Width.Set(400, 0);
			Height.Set(50, 0);

			lootElements.Left.Set(0, 0);
			lootElements.Top.Set(50, 0);
			lootElements.Width.Set(400, 0);
			lootElements.Height.Set(0, 0);
			lootElements.ListPadding = 2f;
			Append(lootElements);

			removeButton.Left.Set(-36, 1);
			removeButton.Top.Set(6, 0);
			removeButton.Width.Set(32, 0);
			removeButton.Height.Set(32, 0);
			removeButton.OnLeftClick += Remove;
			Append(removeButton);

			upButton.Left.Set(8, 0);
			upButton.Top.Set(-3, 0);
			upButton.Width.Set(24, 0);
			upButton.Height.Set(18, 0);
			upButton.SetVisibility(1, 0.8f);
			upButton.OnLeftClick += MoveUp;
			Append(upButton);

			downButton.Left.Set(8, 0);
			downButton.Top.Set(27, 0);
			downButton.Width.Set(24, 0);
			downButton.Height.Set(18, 0);
			downButton.SetVisibility(1, 0.8f);
			downButton.OnLeftClick += MoveDown;
			Append(downButton);

			hideButton.Left.Set(-56, 1);
			hideButton.Top.Set(16, 0);
			hideButton.Width.Set(18, 0);
			hideButton.Height.Set(12, 0);
			hideButton.SetVisibility(1, 0.5f);
			hideButton.OnLeftClick += Hide;
			Append(hideButton);

			foreach (Loot loot in rule.pool)
				AddItem(loot);
		}

		private void Hide(UIMouseEvent evt, UIElement listeningElement)
		{
			if (storedHeight == 0)
			{
				hideButton.SetImage(Assets.GUI.EyeClosed);
				storedHeight = GetDimensions().Height;
				Height.Set(50, 0);
			}
			else
			{
				hideButton.SetImage(Assets.GUI.Eye);
				Height.Set(storedHeight, 0);
				storedHeight = 0;
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

		public override void LeftClick(UIMouseEvent evt)
		{
			if (Main.mouseItem.IsAir || storedHeight > 0)
				return;

			AddItem(Main.mouseItem);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 pos = GetDimensions().ToRectangle().TopLeft();
			var target = new Rectangle((int)pos.X, (int)pos.Y, (int)GetDimensions().Width, 46);
			Helpers.GUIHelper.DrawBox(spriteBatch, target, color);

			if (target.Contains(Main.MouseScreen.ToPoint()))
			{
				Tooltip.SetName(rule.Name);
				Tooltip.SetTooltip("Left click with an item to add to this rule");
			}

			if (removeButton.IsMouseHovering)
			{
				Tooltip.SetName("Remove rule");
				Tooltip.SetTooltip("Removes this rule.");
			}

			if (upButton.IsMouseHovering)
			{
				Tooltip.SetName("Move up");
				Tooltip.SetTooltip("Moves this rule earlier in priority");
			}

			if (downButton.IsMouseHovering)
			{
				Tooltip.SetName("Move down");
				Tooltip.SetTooltip("Moves this rule later in priority");
			}

			if (hideButton.IsMouseHovering)
			{
				Tooltip.SetName("Collapse");
				Tooltip.SetTooltip("Collapse or expand this rule");
			}

			Utils.DrawBorderString(spriteBatch, rule.Name, pos + new Vector2(38, 14), Color.White, 0.9f);

			if (storedHeight == 0)
			{
				base.Draw(spriteBatch);
			}
			else
			{
				removeButton.Draw(spriteBatch);
				upButton.Draw(spriteBatch);
				downButton.Draw(spriteBatch);
				hideButton.Draw(spriteBatch);
			}
		}

		//These handle adding/removing the elements and items from the appropriate lists, as well as re-sizing the element.
		public void AddItem(Item item)
		{
			Loot loot = rule.AddItem(item);

			var element = new LootElement(loot, rule.UsesWeight);
			lootElements.Add(element);
			lootElements.Height.Set(lootElements.Height.Pixels + element.Height.Pixels + 2, 0);
			Height.Set(Height.Pixels + element.Height.Pixels + 2, 0);
		}

		public void AddItem(Loot loot)
		{
			var element = new LootElement(loot, rule.UsesWeight);
			lootElements.Add(element);
			lootElements.Height.Set(lootElements.Height.Pixels + element.Height.Pixels + 2, 0);
			Height.Set(Height.Pixels + element.Height.Pixels + 2, 0);
		}

		public void RemoveItem(Loot loot, LootElement element)
		{
			rule.RemoveItem(loot);
			lootElements.Remove(element);
			lootElements.Height.Set(lootElements.Height.Pixels - element.Height.Pixels - 2, 0);
			Height.Set(Height.Pixels - element.Height.Pixels - 2, 0);
		}

		private void Remove(UIMouseEvent evt, UIElement listeningElement)
		{
			if (!(Parent.Parent.Parent is ChestCustomizerState))
				return;

			var parent = Parent.Parent.Parent as ChestCustomizerState;
			parent.ruleElements.Remove(this);
		}
	}
}