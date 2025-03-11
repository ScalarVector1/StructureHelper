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
using Terraria.ModLoader;

namespace StructureHelper.API
{
	/// <summary>
	/// In this class you will find various utilities related to generating multi structures and getting important
	/// information from them such as their dimensions or structure count.
	/// </summary>
	internal class MultiStructureGenerator
	{
		private static readonly Dictionary<string, MultiStructureData> MultiStructureCache = [];

		/// <summary>
		/// Helper to check bounds on a generation call. You can also use this to check the bounds
		/// of your own structure
		/// </summary>
		/// <param name="data">The MultiStructureData to check</param>
		/// <param name="pos">The position to check from, this would be the top-left of the structure.</param>
		/// <param name="index">The index in the multistructure to check the dimensions of.</param>
		/// <returns>If the structure is in bounds or not</returns>
		public static bool IsInBounds(MultiStructureData data, int index, Point16 pos)
		{
			if (index < 0 || index >= data.count)
				throw new IndexOutOfRangeException(ErrorHelper.GenerateErrorMessage($"Index {index} is out of bounds for a multistructure! Max index is {data.count - 1}. You can use GetStructureCount to check the max index if needed.", null));

			StructureData sData = data.structures[index];

			if (pos.X < 0 || pos.X + sData.width >= Main.maxTilesX || pos.Y < 0 || pos.Y + sData.height >= Main.maxTilesY)
				return false;

			return true;
		}

		/// <summary>
		/// Helper to check bounds on a generation call. You can also use this to check the bounds
		/// of your own structure
		/// </summary>
		/// <param name="path">The path to search for the multi structure file</param>
		/// <param name="mod">The mod to search for the multi structure file in</param>
		/// <param name="pos">The position to check from, this would be the top-left of the structure.</param>
		/// <param name="index">The index in the multistructure to check the dimensions of.</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <returns>If the structure is in bounds or not</returns>
		public static bool IsInBounds(string path, Mod mod, int index, Point16 pos, bool fullPath = false)
		{
			MultiStructureData data = GetMultiStructureData(path, mod, fullPath);

			if (index < 0 || index >= data.count)
				throw new IndexOutOfRangeException(ErrorHelper.GenerateErrorMessage($"Index {index} is out of bounds for the multistructure at {path}! Max index is {data.count - 1}. You can use GetStructureCount to check the max index if needed.", mod));

			StructureData sData = data.structures[index];

			if (pos.X < 0 || pos.X + sData.width >= Main.maxTilesX || pos.Y < 0 || pos.Y + sData.height >= Main.maxTilesY)
				return false;

			return true;
		}

		/// <summary>
		/// Gets the dimensions (width and height) of a structure in a multistructure.
		/// </summary>
		/// <param name="path">The path to search for the structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="index">The index in the multistructure to check the dimensions of.</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <returns></returns>
		public static Point16 GetStructureDimensions(string path, Mod mod, int index, bool fullPath = false)
		{
			MultiStructureData data = GetMultiStructureData(path, mod, fullPath);

			if (index < 0 || index >= data.count)
				throw new IndexOutOfRangeException(ErrorHelper.GenerateErrorMessage($"Index {index} is out of bounds for the multistructure at {path}! Max index is {data.count - 1}. You can use GetStructureCount to check the max index if needed.", mod));

			StructureData sData = data.structures[index];

			return new Point16(sData.width, sData.height);
		}

		/// <summary>
		/// Gets the amount of structures inside of a multi structure file
		/// </summary>
		/// <param name="path">The path to search for the structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <returns></returns>
		public static int GetStructureCount(string path, Mod mod, bool fullPath = false)
		{
			MultiStructureData data = GetMultiStructureData(path, mod, fullPath);
			return data.count;
		}

		/// <summary>
		/// Gets the MultiStructureData for a given path in a given mod (or absolute path if fullPath is true).
		/// Will attempt to retrieve from the MultiStructureData cache first if possible before doing I/O
		/// </summary>
		/// <param name="path">The path to search for the multi structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="mod">The mod to search for the multi structure file in</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <returns>The MultiStructureData associated with the desired file</returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static MultiStructureData GetMultiStructureData(string path, Mod mod, bool fullPath = false)
		{
			if (!path.EndsWith(".shmstruct"))
				path += ".shmstruct";

			string key = Path.Combine(mod.Name, path);

			if (MultiStructureCache.ContainsKey(key))
				return MultiStructureCache[key];

			if (fullPath && !File.Exists(path))
				throw new FileNotFoundException(ErrorHelper.GenerateErrorMessage($"A file at the path {path} does not exist! (did you mean to not pass true to fullPath? You should pass false if the file is in your mod!)", mod));

			if (!fullPath && !mod.FileExists(path))
				throw new FileNotFoundException(ErrorHelper.GenerateErrorMessage($"A file at the path {path} in mod {mod.DisplayName}({mod.Name}) does not exist! Did you accidentally include your mods name in the path? (for example, you should not pass 'MyMod/Structures/House', but rather 'Structures/House')", mod));

			using Stream stream = fullPath ? File.OpenRead(path) : mod.GetFileStream(path);
			using var compressionStream = new GZipStream(stream, CompressionMode.Decompress);
			using BinaryReader reader = new(compressionStream);
			MultiStructureCache.Add(key, MultiStructureData.FromStream(reader));
			return MultiStructureCache[key];
		}

		/// <summary>
		/// This method generates a random structure from a multi structure file.
		/// </summary>
		/// <param name="path">The path to search for the structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="pos">The position in the world to place the top-left of the structure, in tile coordinates.</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll reference.</param>
		/// <param name="flags">Allows you to pass flags for special generation behavior. See <see cref="GenFlags"/> </param>
		public static void GenerateMultistructureRandom(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			MultiStructureData data = GetMultiStructureData(path, mod, fullPath);

			int index;

			if (WorldGen.generatingWorld)
				index = WorldGen.genRand.Next(data.count);
			else
				index = Main.rand.Next(data.count);

			Generator.GenerateFromData(data.structures[index], pos, ignoreNull, flags);
		}

		/// <summary>
		/// Generates a specific structure from a multi structure file
		/// </summary>
		/// <param name="path">The path to search for the structure file. If it is in a mod, it should not include the mods name (for example, it should be "structures/coolHouse", not "CoolHouseMod/structures/coolHouse")</param>
		/// <param name="index">The index of the structure to generate out of the multistructure</param>
		/// <param name="pos">The position in the world to place the top-left of the structure, in tile coordinates.</param>
		/// <param name="mod">The mod to search for the structure file in</param>
		/// <param name="fullPath">If the search path starts at the root of your file system(true) or the provided mod(false). This should usually be false.</param>
		/// <param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll reference.</param>
		/// <param name="flags">Allows you to pass flags for special generation behavior. See <see cref="GenFlags"/> </param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static void GenerateMultistructureSpecific(string path, int index, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false, GenFlags flags = GenFlags.None)
		{
			MultiStructureData data = GetMultiStructureData(path, mod, fullPath);

			if (index < 0 || index >= data.count)
				throw new IndexOutOfRangeException(ErrorHelper.GenerateErrorMessage($"Index {index} is out of bounds for the multistructure at {path}! Max index is {data.count - 1}. You can use GetStructureCount to check the max index if needed.", mod));

			Generator.GenerateFromData(data.structures[index], pos, ignoreNull, flags);
		}
	}
}
