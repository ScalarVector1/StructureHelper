using Microsoft.Xna.Framework;
using StructureHelper.ChestHelper;
using StructureHelper.ChestHelper.GUI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper.Items
{
    class ChestWand : ModItem
    {
        public override bool AltFunctionUse(Player player) => true;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chest Wand");
            Tooltip.SetDefault("Right click to open the chest rule menu\nLeft click a chest to set the current rules on it\nRight click a chest to copy it's rules");
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
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);

            if(tile.type == TileID.Containers)
			{
                int xOff = tile.frameX % 36 / 18;
                int yOff = tile.frameY % 36 / 18;

                TileEntity.PlaceEntityNet(Player.tileTargetX - xOff, Player.tileTargetY - yOff, ModContent.TileEntityType<ChestEntity>());
                StructureHelper.Instance.ChestCustomizer.SetData(TileEntity.ByPosition[new Point16(Player.tileTargetX - xOff, Player.tileTargetY - yOff)] as ChestEntity);
            }

            if(player.altFunctionUse == 2)
                ChestCustomizerState.Visible = !ChestCustomizerState.Visible;



            return true;
        }
    }
}