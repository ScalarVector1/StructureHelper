global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ModLoader;
using StructureHelper.Content.Tiles;
using System.Reflection.Metadata;

namespace StructureHelper
{
	public class StructureHelper : Mod
	{
		public static ushort NullTileID;
		public static ushort NullWallID;

		public const ushort NULL_IDENTIFIER = ushort.MaxValue;

		public StructureHelper() { Instance = this; }

		public static StructureHelper Instance { get; set; }

		public override void PostSetupContent()
		{
			NullTileID = (ushort)ModContent.TileType<NullBlock>();
			NullWallID = (ushort)ModContent.WallType<NullWall>();
		}

		public override void Unload()
		{
			API.Legacy.LegacyGenerator.StructureDataCache.Clear();
			API.Generator.StructureCache.Clear();
			API.MultiStructureGenerator.MultiStructureCache.Clear();
		}
	}
}