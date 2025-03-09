global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ModLoader;

namespace StructureHelper
{
	public class StructureHelper : Mod
	{
		public static ushort NullTileID;
		public static ushort NullWallID;

		public StructureHelper() { Instance = this; }

		public static StructureHelper Instance { get; set; }

		public override void PostSetupContent()
		{
			NullTileID = (ushort)ModContent.TileType<NullBlock>();
			NullWallID = (ushort)ModContent.WallType<NullWall>();
		}

		public override void Unload()
		{
			Generator.StructureDataCache.Clear();
		}
	}
}