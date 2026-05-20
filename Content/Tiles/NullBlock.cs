using Terraria.ID;

namespace StructureHelper.Content.Tiles
{
	class NullBlockFraming : ModSystem
	{
		public override void Load()
		{
			On_WorldGen.SlopeTile += SlopeTileHook;
		}

		private static bool SlopeTileHook(On_WorldGen.orig_SlopeTile orig, int i, int j, int slope, bool noEffects)
		{
			bool isNeighborNull = false;

			isNeighborNull |= Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<NullBlock>();
			isNeighborNull |= Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<NullBlock>();
			isNeighborNull |= Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<NullBlock>();
			isNeighborNull |= Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<NullBlock>();

			if (isNeighborNull)
				return false;

			return orig(i, j, slope, noEffects);
		}
	}

	class NullBlock : ModTile
	{
		public override string Texture => "StructureHelper/Assets/Tiles/" + Name;

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
		}

		public override bool Slope(int i, int j) { return false; }

		public override bool CanDrop(int i, int j) { return false; }
	}

	class NullWall : ModWall
	{
		public override string Texture => "StructureHelper/Assets/Tiles/" + Name;

		public override bool Drop(int i, int j, ref int type) { return false; }
	}

	class NullBlockItem : ModItem
	{
		public override string Texture => "StructureHelper/Assets/Tiles/" + Name;

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 2;
			Item.useTime = 2;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Yellow;
			Item.createTile = ModContent.TileType<NullBlock>();
		}
	}

	class NullWallItem : ModItem
	{
		public override string Texture => "StructureHelper/Assets/Tiles/" + Name;

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 2;
			Item.useTime = 2;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Yellow;
			Item.createWall = ModContent.WallType<NullWall>();
		}
	}

	class NullTileAndWallPlacer : ModItem
	{
		public override string Texture => "StructureHelper/Assets/Tiles/" + Name;

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 2;
			Item.useTime = 2;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Yellow;
			Item.createTile = ModContent.TileType<NullBlock>();
			Item.createWall = ModContent.WallType<NullWall>();
		}
	}
}