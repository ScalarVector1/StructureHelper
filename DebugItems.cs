//#if debug
using Microsoft.Xna.Framework;
using StructureHelper.ChestHelper;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
    class TestChestSaver : ModItem
    {
        public override string Texture => "StructureHelper/icon";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Chest Wand");
            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 20;
            item.useAnimation = 20;
            item.rare = 1;
        }

        public override bool UseItem(Player player)
        {
            ChestEntity test = new ChestEntity();

            var rule = new ChestRuleGuaranteed();
            Item item = new Item();
            item.SetDefaults(ItemID.TwilightDye);

            rule.pool.Add(new Loot(item, 1));

            test.rules.Add(rule);

            test.SaveChestRulesFile();
            return true;
        }
    }

    class TestChestPlace : ModItem
    {
        public override string Texture => "StructureHelper/icon";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Chest Placer");
            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 20;
            item.useAnimation = 20;
            item.rare = 1;
        }

        public override bool UseItem(Player player)
        {
            StructureHelper.GenerateChest("SampleChest", (Main.MouseWorld / 16).ToPoint16(), mod, TileID.Containers);
            return true;
        }
    }
}
//#endif