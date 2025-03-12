using StructureHelper.ChestHelper;
using StructureHelper.Content.GUI.ChestGUI;
using StructureHelper.Core.Loaders.UILoading;
using Terraria.DataStructures;
using Terraria.ID;

namespace StructureHelper.Content.Items
{
	class ChestWand : ModItem
	{
		public override string Texture => "StructureHelper/Assets/Items/" + Name;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chest Wand");
			Tooltip.SetDefault("Right click to open the chest rule menu\nLeft click a chest to set the current rules on it\nRight click a chest with rules to copy them");
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);

			if (tile.TileType == TileID.Containers || TileID.Sets.BasicChest[tile.TileType])
			{
				int xOff = tile.TileFrameX % 36 / 18;
				int yOff = tile.TileFrameY % 36 / 18;

				if (player.altFunctionUse == 2)
				{
					if (TileEntity.ByPosition.ContainsKey(new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)))
					{
						var chestEntity = TileEntity.ByPosition[new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)] as ChestEntity;

						UILoader.GetUIState<ChestCustomizerState>().ruleElements.Clear();

						for (int k = 0; k < chestEntity.rules.Count; k++)
						{
							ChestRule rule = chestEntity.rules[k].Clone();

							var elem = new ChestRuleElement(rule);

							if (rule is ChestRuleGuaranteed)
								elem = new GuaranteedRuleElement(rule);

							if (rule is ChestRuleChance)
								elem = new ChanceRuleElement(rule);

							if (rule is ChestRulePool)
								elem = new PoolRuleElement(rule);

							if (rule is ChestRulePoolChance)
								elem = new PoolChanceRuleElement(rule);

							UILoader.GetUIState<ChestCustomizerState>().ruleElements.Add(elem);
						}
					}
					else
					{
						UILoader.GetUIState<ChestCustomizerState>().ruleElements.Clear();
					}

					Main.NewText($"Copied chest rules from chest at {new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)}");
				}
				else
				{
					bool overwrite = TileEntity.ByPosition.ContainsKey(new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff));

					TileEntity.PlaceEntityNet(Player.tileTargetX - xOff, Player.tileTargetY - yOff, ModContent.TileEntityType<ChestEntity>());
					bool cleared = !UILoader.GetUIState<ChestCustomizerState>().SetData(TileEntity.ByPosition[new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)] as ChestEntity);

					if (overwrite)
					{
						if (cleared)
							Main.NewText($"Removed chest rules for chest at {new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)}", Color.Orange);
						else
							Main.NewText($"Overwritten chest rules for chest at {new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)}", Color.Yellow);
					}
					else if (!cleared)
					{
						Main.NewText($"Set chest rules for chest at {new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)}", Color.GreenYellow);
					}
				}
			}

			if (player.altFunctionUse == 2)
				UILoader.GetUIState<ChestCustomizerState>().Visible = true;

			return true;
		}
	}
}