using StructureHelper.Helpers;
using StructureHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace StructureHelper.Generating
{
	internal class Generator
	{
		private static readonly Dictionary<string, StructureData> StructureCache = new();

		/// <summary>
		/// Helper to check bounds on a generation call
		/// </summary>
		/// <param name="tag">The tag to check</param>
		/// <param name="pos">The position to check from</param>
		/// <returns>If the structure is in bounds or not</returns>
		private static bool IsInBounds(StructureData data, Point16 pos)
		{
			if (pos.X < 0 || pos.X + data.width >= Main.maxTilesX || pos.Y < 0 || pos.Y + data.height >= Main.maxTilesY)
				return false;

			return true;
		}

		private static StructureData GetData(string path, Mod mod, bool fullPath = false)
		{
			var key = Path.Combine(mod.Name, path);

			if (StructureCache.ContainsKey(key))
				return StructureCache[key];

			if (fullPath && !File.Exists(path))
				throw new FileNotFoundException($"A file at the path {path} does not exist! (did you mean to not pass true to fullPath? You should pass false if the file is in your mod!)");

			if (!fullPath && mod.FileExists(path))
				throw new FileNotFoundException($"A file at the path {path} in mod {mod.DisplayName}({mod.Name}) does not exist! Did you accidentally include your mods name in the path? (for example, you should not pass 'MyMod/Structures/House', but rather 'Structures/House')");

			using Stream stream = fullPath ? File.OpenRead(path) : mod.GetFileStream(path);
			using GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress);
			using BinaryReader reader = new(compressionStream);
			StructureCache.Add(key, StructureData.FromStream(reader));
			return StructureCache[key];
		}

		public static void GenerateStructure(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			StructureData data = GetData(path, mod, fullPath);

			if (!IsInBounds(data, pos))
				throw new ArgumentException(ErrorHelper.GenerateErrorMessage($"Attempted to generate a structure out of bounds! {pos} is not a valid position for the structure at {path}. Mods are responsible for bounds-checking their own structures. You can fetch dimension data using GetDimensions or GetMultistructureDimensions.", mod));

			Generate(data, pos, ignoreNull, flags);
		}

		public static void Generate(StructureData data, Point16 pos, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			for(int k = 0; k < data.width; k++)
			{
				data.ExportDataColumn<TileTypeData>(pos.X + k, pos.Y, k, null);
				data.ExportDataColumn<WallTypeData>(pos.X + k, pos.Y, k, null);
				data.ExportDataColumn<LiquidData>(pos.X + k, pos.Y, k, null);
				data.ExportDataColumn<TileWallBrightnessInvisibilityData>(pos.X + k, pos.Y, k, null);
				data.ExportDataColumn<TileWallWireStateData>(pos.X + k, pos.Y, k, null);
			}
		}
	}
}
