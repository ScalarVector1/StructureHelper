using Microsoft.Xna.Framework;
using StructureHelper.GUI;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace StructureHelper.Items
{
	class TestWand : ModItem
	{
		public static bool ignoreNulls = false;
		public static bool UIVisible;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Structure Placer Wand");
			Tooltip.SetDefault("left click to place the selected structure, right click to open the structure selector");
		}

		public override void SetDefaults()
		{
			Item.useStyle = 1;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = 1;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				UIVisible = !UIVisible;
				return true;
			}

			if (ManualGeneratorMenu.selected != null)
			{
				var pos = new Point16(Player.tileTargetX, Player.tileTargetY);

				if (ManualGeneratorMenu.multiMode)
					Generator.GenerateMultistructureSpecific(ManualGeneratorMenu.selected.path, pos, StructureHelper.Instance, ManualGeneratorMenu.multiIndex, true, ManualGeneratorMenu.ignoreNulls);
				else
					Generator.GenerateStructure(ManualGeneratorMenu.selected.path, pos, StructureHelper.Instance, true, ManualGeneratorMenu.ignoreNulls);
			}
			else
			{
				Main.NewText("No structure selected! Right click and select a structure from the menu to generate it.", Color.Red);
			}

			return true;
		}
	}
}
