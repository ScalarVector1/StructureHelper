using StructureHelper.ChestHelper;
using StructureHelper.Core.Loaders.UILoading;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace StructureHelper.Content.GUI.ChestGUI
{
	class ChestCustomizerState : SmartUIState
	{
		internal UIList ruleElements = [];
		internal UIScrollbar scrollBar = new();

		readonly UIImageButton NewGuaranteed = new(ModContent.Request<Texture2D>("StructureHelper/GUI/PlusR"));
		readonly UIImageButton NewChance = new(ModContent.Request<Texture2D>("StructureHelper/GUI/PlusG"));
		readonly UIImageButton NewPool = new(ModContent.Request<Texture2D>("StructureHelper/GUI/PlusP"));
		readonly UIImageButton NewPoolChance = new(ModContent.Request<Texture2D>("StructureHelper/GUI/PlusB"));

		public static UIImageButton closeButton = new(ModContent.Request<Texture2D>("StructureHelper/GUI/Cross"));

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public override void OnInitialize()
		{
			ManualGeneratorMenu.SetDims(ruleElements, -200, 0.5f, 0, 0.1f, 400, 0, 0, 0.8f);
			ManualGeneratorMenu.SetDims(scrollBar, 232, 0.5f, 0, 0.1f, 32, 0, 0, 0.8f);
			ruleElements.SetScrollbar(scrollBar);
			ruleElements.ListPadding = 10f;
			Append(ruleElements);
			Append(scrollBar);

			ManualGeneratorMenu.SetDims(NewGuaranteed, -200, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			NewGuaranteed.OnLeftClick += (n, m) => ruleElements.Add(new GuaranteedRuleElement());
			Append(NewGuaranteed);

			ManualGeneratorMenu.SetDims(NewChance, -160, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			NewChance.OnLeftClick += (n, m) => ruleElements.Add(new ChanceRuleElement());
			Append(NewChance);

			ManualGeneratorMenu.SetDims(NewPool, -120, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			NewPool.OnLeftClick += (n, m) => ruleElements.Add(new PoolRuleElement());
			Append(NewPool);

			ManualGeneratorMenu.SetDims(NewPoolChance, -80, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			NewPoolChance.OnLeftClick += (n, m) => ruleElements.Add(new PoolChanceRuleElement());
			Append(NewPoolChance);

			ManualGeneratorMenu.SetDims(closeButton, 200 - 32, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			closeButton.OnLeftClick += (n, m) => Visible = false;
			Append(closeButton);
		}

		public bool SetData(ChestEntity entity)
		{
			entity.rules.Clear();

			if (ruleElements.Count == 0)
			{
				entity.Kill(entity.Position.X, entity.Position.Y);
				return false;
			}

			for (int k = 0; k < ruleElements.Count; k++)
			{
				entity.rules.Add((ruleElements._items[k] as ChestRuleElement).rule.Clone());
			}

			return true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Recalculate();

			var color = new Color(49, 84, 141);

			Helpers.GUIHelper.DrawBox(spriteBatch, NewGuaranteed.GetDimensions().ToRectangle(), NewGuaranteed.IsMouseHovering ? color : color * 0.8f);
			Helpers.GUIHelper.DrawBox(spriteBatch, NewChance.GetDimensions().ToRectangle(), NewChance.IsMouseHovering ? color : color * 0.8f);
			Helpers.GUIHelper.DrawBox(spriteBatch, NewPool.GetDimensions().ToRectangle(), NewPool.IsMouseHovering ? color : color * 0.8f);
			Helpers.GUIHelper.DrawBox(spriteBatch, NewPoolChance.GetDimensions().ToRectangle(), NewPoolChance.IsMouseHovering ? color : color * 0.8f);

			Helpers.GUIHelper.DrawBox(spriteBatch, closeButton.GetDimensions().ToRectangle(), closeButton.IsMouseHovering ? color : color * 0.8f);

			var rect = ruleElements.GetDimensions().ToRectangle();
			rect.Inflate(30, 10);
			Helpers.GUIHelper.DrawBox(spriteBatch, rect, new Color(20, 40, 60) * 0.8f);

			if (rect.Contains(Main.MouseScreen.ToPoint()))
				Main.LocalPlayer.mouseInterface = true;

			if (NewGuaranteed.IsMouseHovering)
			{
				Tooltip.SetName("New 'Guaranteed' rule");
				Tooltip.SetTooltip("Guaranteed rules will always place every item in them in this chest, in the order they appear in the rule.");
			}

			if (NewChance.IsMouseHovering)
			{
				Tooltip.SetName("New 'Chance' rule");
				Tooltip.SetTooltip("Chance rules give all items in the rule a chance to generate. You can customize the value for this chance from 0% to 100%");
			}

			if (NewPool.IsMouseHovering)
			{
				Tooltip.SetName("New 'Pool' rule");
				Tooltip.SetTooltip("Pool rules will select a customizable amount of items from them and place them in the chest.");
			}

			if (NewPoolChance.IsMouseHovering)
			{
				Tooltip.SetName("New 'Pool + Chance' rule");
				Tooltip.SetTooltip("Pool + Chance rules act as a combination of a chance and pool rule -- they have a customizable chance to occur, and if they do, act as a pool rule.");
			}

			if (closeButton.IsMouseHovering)
			{
				Tooltip.SetName("Close");
				Tooltip.SetTooltip("Close this menu");
			}

			base.Draw(spriteBatch);
		}
	}
}