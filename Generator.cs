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
	/// <summary>
	/// A static class providing utilities to generate structures.
	/// </summary>
	public static class Generator
	{
		internal static Dictionary<string, TagCompound> StructureDataCache = new();

		/// <summary>
		/// This method generates a structure from a structure file within your mod.
		/// </summary>
		/// <param name="path">The path to your structure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="pos">The position in the world in which you want your structure to generate, in tile coordinates.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		///<param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		///<param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll reference.</param>
		public static bool GenerateStructure(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false)
		{
			TagCompound tag = GetTag(path, mod, fullPath);

			if (!tag.ContainsKey("Version") || tag.GetString("Version")[0] <= 1)
				throw new Exception("Legacy structures from 1.3 versions of this mod are not supported.");

			if (tag.ContainsKey("Structures"))
				throw new Exception($"Attempted to generate a multistructure '{path}' as a structure. Use GenerateMultistructureRandom or GenerateMultistructureSpecific instead.");

			return Generate(tag, pos, ignoreNull);
		}

		/// <summary>
		/// This method generates a structure selected randomly from a multistructure file within your mod.
		/// </summary>
		/// <param name="path">The path to your multistructure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="pos">The position in the world in which you want your structure to generate, in tile coordinates.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		///<param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		///<param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll refference.</param>
		public static bool GenerateMultistructureRandom(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false)
		{
			TagCompound tag = GetTag(path, mod, fullPath);

			if (!tag.ContainsKey("Version") || tag.GetString("Version")[0] <= 1)
				throw new Exception("Legacy structures from 1.3 versions of this mod are not supported.");

			if (!tag.ContainsKey("Structures"))
				throw new Exception($"Attempted to generate a structure '{path}' as a multistructure. use GenerateStructure instead.");

			var structures = (List<TagCompound>)tag.GetList<TagCompound>("Structures");
			int index = WorldGen.genRand.Next(structures.Count);
			TagCompound targetStructure = structures[index];

			return Generate(targetStructure, pos, ignoreNull);
		}

		/// <summary>
		/// This method generates a structure you select from a multistructure file within your mod. Useful if you want to do your own weighted randomization or want additional logic based on dimensions gotten from GetMultistructureDimensions.
		/// </summary>
		/// <param name="path">The path to your multistructure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="pos">The position in the world in which you want your structure to generate, in tile coordinates.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="index">The index of the structure you want to generate out of the multistructure file, structure indicies are 0-based and match the order they were saved in.</param>
		///<param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		///<param name="ignoreNull">If the structure should repsect the normal behavior of null tiles or not. This should never be true if you're using the mod as a dll refference.</param>
		public static bool GenerateMultistructureSpecific(string path, Point16 pos, Mod mod, int index, bool fullPath = false, bool ignoreNull = false)
		{
			TagCompound tag = GetTag(path, mod, fullPath);

			if (!tag.ContainsKey("Version") || tag.GetString("Version")[0] <= 1)
				throw new Exception("Legacy structures from 1.3 versions of this mod are not supported.");

			var structures = (List<TagCompound>)tag.GetList<TagCompound>("Structures");

			if (index >= structures.Count || index < 0)
			{
				StructureHelper.Instance.Logger.Warn($"Attempted to generate structure {index} in mutistructure containing {structures.Count - 1} structures.");
				return false;
			}

			TagCompound targetStructure = structures[index];

			return Generate(targetStructure, pos, ignoreNull);
		}

		/// <summary>
		/// Gets the dimensions of a structure from a structure file within your mod.
		/// </summary>
		/// <param name="path">The path to your structure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="dims">The Point16 variable which you want to be set to the dimensions of the structure.</param>
		/// <param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		/// <returns></returns>
		public static bool GetDimensions(string path, Mod mod, ref Point16 dims, bool fullPath = false)
		{
			TagCompound tag = GetTag(path, mod, fullPath);

			dims = new Point16(tag.GetInt("Width"), tag.GetInt("Height"));
			return true;
		}

		/// <summary>
		/// Gets the dimensions of a structure from a structure file within your mod.
		/// </summary>
		/// <param name="path">The path to your structure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <param name="index">The index of the structure you want to get the dimensions of out of the multistructure file, structure indicies are 0-based and match the order they were saved in.</param>
		/// <param name="dims">The Point16 variable which you want to be set to the dimensions of the structure.</param>
		/// <param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only be used for debugging.</param>
		/// <returns></returns>
		public static bool GetMultistructureDimensions(string path, Mod mod, int index, ref Point16 dims, bool fullPath = false)
		{
			TagCompound tag = GetTag(path, mod, fullPath);

			var structures = (List<TagCompound>)tag.GetList<TagCompound>("Structures");

			if (index >= structures.Count || index < 0)
			{
				dims = new Point16(0, 0);
				StructureHelper.Instance.Logger.Warn($"Attempted to get dimensions of structure {index} in mutistructure containing {structures.Count - 1} structures.");
				return false;
			}

			TagCompound targetStructure = structures[index];

			dims = new Point16(targetStructure.GetInt("Width"), targetStructure.GetInt("Height"));
			return true;
		}

		/// <summary>
		/// Checks if a structure file is a multistructure or not. Can be used to easily add support for parameterizing strucutres or multistructures in your mod.
		/// </summary>
		/// <param name="path">The path to the structure file you wish to check.</param>
		/// <param name="mod">The instance of your mod to grab the file from.</param>
		/// <returns>True if the file is a multistructure, False if the file is a structure, null if it is invalid.</returns>
		public static bool? IsMultistructure(string path, Mod mod)
		{
			TagCompound tag = GetTag(path, mod);

			if (tag is null)
				return null;

			if (tag.ContainsKey("Structures"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Parses and generates the actual tiles from a structure file
		/// </summary>
		/// <param name="tag">The structure data TagCompound to generate from</param>
		/// <param name="pos">The position in the world of the top-leftmost tile to be placed at</param>
		/// <param name="ignoreNull">If this structure should place null tiles or not</param>
		/// <returns>If the structure successfully generated or not</returns>
		public static unsafe bool Generate(TagCompound tag, Point16 pos, bool ignoreNull = false)
		{
			var data = (List<TileSaveData>)tag.GetList<TileSaveData>("TileData");

			if (data is null)
			{
				StructureHelper.Instance.Logger.Warn("Corrupt or Invalid structure data.");
				return false;
			}

			int width = tag.GetInt("Width");
			int height = tag.GetInt("Height");

			for (int x = 0; x <= width; x++)
			{
				for (int y = 0; y <= height; y++)
				{
					int index = y + x * (height + 1);
					TileSaveData d = data[index];
					Tile tile = Framing.GetTileSafely(pos.X + x, pos.Y + y);

					bool isNullTile = false;

					bool isNullWall = false;
					ushort oldWall = tile.WallType; //Saved incase there is a null wall and we need to restore

					if (!int.TryParse(d.tile, out int type))
					{
						string[] parts = d.tile.Split();

						if (parts[0] == "StructureHelper" && parts[1] == "NullBlock" && !ignoreNull)
							isNullTile = true;
						else if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind<ModTile>(parts[1], out ModTile modTileType))
							type = modTileType.Type;
						else
							type = 0;
					}

					if (!int.TryParse(d.wall, out int wallType))
					{
						string[] parts = d.wall.Split();

						if (parts[0] == "StructureHelper" && parts[1] == "NullWall" && !ignoreNull)
							isNullWall = true;
						else if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind<ModWall>(parts[1], out ModWall modWallType))
							wallType = modWallType.Type;
						else
							wallType = 0;
					}

					if (!d.Active)
						isNullTile = false;

					if (!isNullTile || ignoreNull) //leave everything else about the tile alone if its a null block
					{
						tile.ClearEverything();
						tile.TileType = (ushort)type;
						tile.TileFrameX = d.frameX;
						tile.TileFrameY = d.frameY;

						fixed (void* ptr = &tile.Get<TileWallWireStateData>())
						{
							int* intPtr = (int*)ptr;
							intPtr++;

							*intPtr = d.wallWireData;
						}

						fixed (void* ptr = &tile.Get<LiquidData>())
						{
							short* shortPtr = (short*)ptr;

							*shortPtr = d.packedLiquidData;
						}

						if (!d.Active)
							tile.HasTile = false;

						if (d.TEType != "") //place and load a tile entity
						{
							if (d.TEType != "")
							{
								if (d.TEType == "StructureHelper ChestEntity" && !ignoreNull)
								{
									GenerateChest(new Point16(pos.X + x, pos.Y + y), d.TEData);
								}
								else
								{

									if (!int.TryParse(d.TEType, out int typ))
									{
										string[] parts = d.TEType.Split();
										typ = ModLoader.GetMod(parts[0]).Find<ModTileEntity>(parts[1]).Type;
									}

									TileEntity.PlaceEntityNet(pos.X + x, pos.Y + y, typ);

									if (d.TEData != null && typ > 2)
										(TileEntity.ByPosition[new Point16(pos.X + x, pos.Y + y)] as ModTileEntity).LoadData(d.TEData);
								}
							}
						}
						else if ((type == TileID.Containers || TileID.Sets.BasicChest[tile.TileType]) && d.frameX % 36 == 0 && d.frameY % 36 == 0) //generate an empty chest if there is no chest data
						{
							Chest.CreateChest(pos.X + x, pos.Y + y);
						}
					}

					if (!isNullWall || ignoreNull)
						tile.WallType = (ushort)wallType;
					else
						tile.WallType = oldWall; //revert to old wall if its a null wall and not a null tile, else would be overridden by the WallWireData struct
				}
			}

			return true;
		}

		/// <summary>
		/// Places a chest in the world and fills it according to a set of chest rules
		/// </summary>
		/// <param name="pos">The position of the top-leftmost corner of the chest</param>
		/// <param name="rules">The TagCompound containing the chest rules you want to generate your chest with</param>
		internal static void GenerateChest(Point16 pos, TagCompound rules)
		{
			int i = Chest.CreateChest(pos.X, pos.Y);

			if (i == -1)
				return;

			Chest chest = Main.chest[i];
			ChestEntity.SetChest(chest, ChestEntity.LoadChestRules(rules));
		}

		/// <summary>
		/// Loads and caches a structure file.
		/// </summary>
		/// <param name="path">The path to the struture file to load</param>
		/// <param name="mod">The mod to load the structure file from</param>
		/// <param name="fullPath">If the given path is fully qualified</param>
		/// <returns>If the file could successfully be loaded or not</returns>
		internal static bool LoadFile(string path, Mod mod, bool fullPath = false)
		{
			TagCompound tag;

			if (!fullPath)
			{
				System.IO.Stream stream = mod.GetFileStream(path);
				tag = TagIO.FromStream(stream);
				stream.Close();
			}
			else
			{
				tag = TagIO.FromFile(path);
			}

			if (tag is null)
			{
				StructureHelper.Instance.Logger.Warn("Structure was unable to be found. Are you passing the correct path?");
				return false;
			}

			StructureDataCache.Add(path, tag);
			return true;
		}

		/// <summary>
		/// Attempts to get data from a structure/multistructure file. If the data is not cached, it will be loaded and cached.
		/// </summary>
		/// <param name="path">The path of the file to retrieve data from</param>
		/// <param name="mod">The mod to load the structure file from</param>
		/// <param name="fullPath">If the given path is fully qualified</param>
		/// <returns>The TagCompound containing the structure/multistructure data</returns>
		internal static TagCompound GetTag(string path, Mod mod, bool fullPath = false)
		{
			TagCompound tag;

			if (!StructureDataCache.ContainsKey(path))
			{
				if (!LoadFile(path, mod, fullPath))
					return null;

				tag = StructureDataCache[path];
			}
			else
			{
				tag = StructureDataCache[path];
			}

			return tag;
		}
	}
}
