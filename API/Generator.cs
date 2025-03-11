using StructureHelper.Helpers;
using StructureHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Terraria.DataStructures;
using Terraria.ID;

namespace StructureHelper.API
{
	/// <summary>
	/// In this class you will find various utilities related to generating structures and getting important
	/// information from them such as their dimensions.
	/// </summary>
	public class Generator
	{
		private static readonly Dictionary<string, StructureData> StructureCache = [];

		/// <summary>
		/// Helper to check bounds on a generation call. You can also use this to check the bounds
		/// of your own structure
		/// </summary>
		/// <param name="data">The StructureData to check</param>
		/// <param name="pos">The position to check from, this would be the top-left of the structure.</param>
		/// <returns>If the structure is in bounds or not</returns>
		public static bool IsInBounds(StructureData data, Point16 pos)
		{
			if (pos.X < 0 || pos.X + data.width >= Main.maxTilesX || pos.Y < 0 || pos.Y + data.height >= Main.maxTilesY)
				return false;

			return true;
		}

		/// <summary>
		/// Helper to check bounds on a generation call. You can also use this to check the bounds
		/// of your own structure
		/// </summary>
		/// <param name="path">The path to search for the structure file</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="pos">The position to check from, this would be the top-left of the structure.</param>
		/// <returns>If the structure is in bounds or not</returns>
		public static bool IsInBounds(string path, Mod mod, Point16 pos, bool fullPath = false)
		{
			StructureData data = GetStructureData(path, mod, fullPath);

			if (pos.X < 0 || pos.X + data.width >= Main.maxTilesX || pos.Y < 0 || pos.Y + data.height >= Main.maxTilesY)
				return false;

			return true;
		}

		/// <summary>
		/// Gets the dimensions (width and height) of a structure.
		/// </summary>
		/// <param name="path">The path to search for the structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <returns></returns>
		public static Point16 GetStructureDimensions(string path, Mod mod, bool fullPath = false)
		{
			StructureData data = GetStructureData(path, mod, fullPath);

			return new Point16(data.width, data.height);
		}

		/// <summary>
		/// Gets the StructureData for a given path in a given mod (or absolute path if fullPath is true).
		/// Will attempt to retrieve from the StructureData cache first if possible before doing I/O
		/// </summary>
		/// <param name="path">The path to search for the structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <returns>The StructureData associated with the desired file</returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static StructureData GetStructureData(string path, Mod mod, bool fullPath = false)
		{
			if (!path.EndsWith(".shstruct"))
				path += ".shstruct";

			string key = Path.Combine(mod.Name, path);

			if (StructureCache.ContainsKey(key))
				return StructureCache[key];

			if (fullPath && !File.Exists(path))
				throw new FileNotFoundException(ErrorHelper.GenerateErrorMessage($"A file at the path {path} does not exist! (did you mean to not pass true to fullPath? You should pass false if the file is in your mod!)", mod));

			if (!fullPath && !mod.FileExists(path))
				throw new FileNotFoundException(ErrorHelper.GenerateErrorMessage($"A file at the path {path} in mod {mod.DisplayName}({mod.Name}) does not exist! Did you accidentally include your mods name in the path? (for example, you should not pass 'MyMod/Structures/House', but rather 'Structures/House')", mod));

			using Stream stream = fullPath ? File.OpenRead(path) : mod.GetFileStream(path);
			using var compressionStream = new GZipStream(stream, CompressionMode.Decompress);
			using BinaryReader reader = new(compressionStream);
			StructureCache.Add(key, StructureData.FromStream(reader));
			return StructureCache[key];
		}

		/// <summary>
		/// This method generates a structure from a structure file.
		/// </summary>
		/// <param name="path">The path to search for the structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="pos">The position in the world to place the top-left of the structure, in tile coordinates.</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll reference.</param>
		/// <param name="flags">Allows you to pass flags for special generation behavior. See <see cref="GenFlags"/> </param>
		public static void GenerateStructure(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			StructureData data = GetStructureData(path, mod, fullPath);

			if (!IsInBounds(data, pos))
				throw new ArgumentException(ErrorHelper.GenerateErrorMessage($"Attempted to generate a structure out of bounds! {pos} is not a valid position for the structure at {path}. Mods are responsible for bounds-checking their own structures. You can fetch dimension data using GetDimensions or GetMultistructureDimensions.", mod));

			GenerateFromData(data, pos, ignoreNull, flags);
		}

		/// <summary>
		/// Directly generates a given StructureData into the world. Use this directly if you have
		/// a StructureData object in memory you want to generate from.
		/// </summary>
		/// <param name="data">The StructureData to generate</param>
		/// <param name="pos">The position in the world to place the top-left of the structure, in tile coordinates.</param>
		/// <param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll reference.</param>
		/// <param name="flags">Allows you to pass flags for special generation behavior. See <see cref="GenFlags"/> </param>
		public static void GenerateFromData(StructureData data, Point16 pos, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			if (!IsInBounds(data, pos))
				throw new ArgumentException(ErrorHelper.GenerateErrorMessage($"Attempted to generate a structure out of bounds! {pos} is not a valid position for the structure. Mods are responsible for bounds-checking their own structures. You can fetch dimension data using GetDimensions or GetMultistructureDimensions.", null));

			for (int k = 0; k < data.width; k++)
			{
				if (!data.slowColumns[k] || ignoreNull)
				{
					data.ExportDataColumn<TileTypeData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumn<WallTypeData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumn<LiquidData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumn<TileWallBrightnessInvisibilityData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumn<TileWallWireStateData>(pos.X + k, pos.Y, k, null);
				}
				else
				{
					data.ExportDataColumnSlow<TileTypeData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumnSlow<WallTypeData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumnSlow<LiquidData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumnSlow<TileWallBrightnessInvisibilityData>(pos.X + k, pos.Y, k, null);
					data.ExportDataColumnSlow<TileWallWireStateData>(pos.X + k, pos.Y, k, null);
				}
			}

			// replace nulls from the placeholder null value to the current null ID
			if (ignoreNull)
			{
				for(int x = 0; x < data.width; x++)
				{
					for(int y = 0; y < data.height; y++)
					{
						Tile tile = Main.tile[pos.X + x, pos.Y + y];

						if (tile.TileType == StructureHelper.NULL_IDENTIFIER)
							tile.TileType = StructureHelper.NullTileID;

						if (tile.WallType == StructureHelper.NULL_IDENTIFIER)
							tile.WallType = StructureHelper.NullWallID;
					}
				}
			}

			// Handle the NBT data if this structure is marked as having any
			if (data.containsNbt)
			{
				for (int k = 0; k < data.nbtData.Count; k++)
				{
					StructureNBTEntry thisNbt = data.nbtData[k];
					thisNbt.OnGenerate(pos + new Point16(thisNbt.x, thisNbt.y), ignoreNull, flags);
				}
			}

			// If we're not in worldgen, we should call frame tile to clean up the edges
			// and then sync if we're in multiplayer
			if (!WorldGen.generatingWorld)
			{
				for (int x = 0; x < data.width; x++)
				{
					WorldGen.TileFrame(pos.X + x, pos.Y);
					WorldGen.TileFrame(pos.X + x, pos.Y + data.height);

					WorldGen.SquareWallFrame(pos.X + x, pos.Y);
					WorldGen.SquareWallFrame(pos.X + x, pos.Y + data.height);
				}

				for (int y = 0; y < data.height; y++)
				{
					WorldGen.TileFrame(pos.X, pos.Y + y);
					WorldGen.TileFrame(pos.X + data.width, pos.Y + y);

					WorldGen.SquareWallFrame(pos.X, pos.Y + y);
					WorldGen.SquareWallFrame(pos.X + data.width, pos.Y + y);
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(-1, pos.X, pos.Y, data.width, data.height);
			}
		}
	}
}
