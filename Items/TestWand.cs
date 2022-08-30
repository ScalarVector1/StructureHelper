using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StructureHelper.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper.Items
{
    internal class TestWand : ModItem
	{
        public static bool ignoreNulls = false;
        public static bool UIVisible;

        public override bool AltFunctionUse(Player player) => true;

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

        public override bool? UseItem(Player player)
        {
            if(player.altFunctionUse == 2)
			{
                UIVisible = !UIVisible;
                return true;
			}

            if (ManualGeneratorMenu.selected != null)
            {
                var pos = new Point16(Player.tileTargetX, Player.tileTargetY);

                if (ManualGeneratorMenu.multiMode)
                    Generator.GenerateMultistructureSpecific(ManualGeneratorMenu.selected.Path, pos, StructureHelper.Instance, ManualGeneratorMenu.multiIndex, true, ManualGeneratorMenu.ignoreNulls);

                else
                    Generator.GenerateStructure(ManualGeneratorMenu.selected.Path, pos, StructureHelper.Instance, true, ManualGeneratorMenu.ignoreNulls);
            }
            else
                Main.NewText("No structure selected! Right click and select a structure from the menu to generate it.", Color.Red);

            return true;
        }
    }
}
