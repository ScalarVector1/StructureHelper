global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ModLoader;
using StructureHelper.Content.Tiles;
using StructureHelper.Core;
using System;

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

		public override object Call(params object[] args)
		{
			if (args[0] is string callCode)
			{
				if (callCode == "RegisterCustomData" && args.Length == 3 && args[1] is Type type && args[2] is Mod mod)
				{
					WandSavingSettings.RegisterCustomTypeForSaving(type, mod);
					return null;
				}
				else
				{
					throw new System.ArgumentException("Invalid call arguments! Please see the documentation for valid call arguments.");
				}
			}
			else
			{
				throw new System.ArgumentException("First parameter to any StructureHelper call must be a valid call code string!");
			}
		}
	}
}