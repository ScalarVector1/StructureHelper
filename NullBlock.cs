using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StructureHelper
{
    class NullBlock : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
        }
    }
    class NullWall : ModWall { }

    class NullBlockItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Null Block");
            Tooltip.SetDefault("Use these in a structure to indicate where the generator\n should leave whatever already exists in the world untouched\n ignores walls, use null walls for that :3");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 2;
            item.useTime = 2;
            item.useStyle = 1;
            item.rare = 1;
            item.createTile = ModContent.TileType<NullBlock>();
        }
    }

    class NullWallItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Null Wall");
            Tooltip.SetDefault("Use these in a structure to indicate where the generator\n should leave walls that already exists in the world untouched\n for walls only, use null blocks for other things");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 2;
            item.useTime = 2;
            item.useStyle = 1;
            item.rare = 1;
            item.createWall = ModContent.WallType<NullWall>();
        }
    }

    class NullBoth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Null Tile Place-O-Matic");
            Tooltip.SetDefault("Places a null tile and null wall at the same time!");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 2;
            item.useTime = 2;
            item.useStyle = 1;
            item.rare = 1;
            item.createTile = ModContent.TileType<NullBlock>();
            item.createWall = ModContent.WallType<NullWall>();
        }
    }
}
