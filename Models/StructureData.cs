using Microsoft.Xna.Framework.Input;
using StructureHelper.ChestHelper;
using StructureHelper.Helpers;
using StructureHelper.Models.NbtEntries;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StructureHelper.Models
{
	/// <summary>
	/// Represents the contents of a structure file in-memory. This object is only valid
	/// for a given mod load instance, as modded tile types will be parsed out from the table
	/// into the body data. If you are storing these and your mod reloads, you should consider
	/// them invalid afterwards.
	/// </summary>
	public unsafe class StructureData
	{
		/// <summary>
		/// Header text to denote that this binary file is a SH structure
		/// </summary>
		public const string HEADER_TEXT = "STRUCTURE_HELPER_STRUCTURE";

		/// <summary>
		/// The width of the structure
		/// </summary>
		public int width;

		/// <summary>
		/// The height of the structure
		/// </summary>
		public int height;

		/// <summary>
		/// If the structure contains NBT data or not
		/// </summary>
		public bool containsNbt;

		/// <summary>
		/// If the structure contains custom ITileData entries
		/// </summary>
		public bool containsCustomTileData;

		/// <summary>
		/// The version this structure was saved in.
		/// </summary>
		public Version version;

		/// <summary>
		/// Represents the mapping between the stored tile type and the actual ID of the modded tiles.
		/// There is a table of fully qualified names in the structure file that is used to populate
		/// this when it is loaded.
		/// </summary>
		public Dictionary<ushort, ushort> moddedTileTable = [];

		/// <summary>
		/// Represents the mapping between the stored wall type and the actual ID of the modded walls.
		/// There is a table of fully qualified names in the structure file that is used to populate
		/// this when it is loaded.
		/// </summary>
		public Dictionary<ushort, ushort> moddedWallTable = [];

		/// <summary>
		/// Represents columns which contain null tiles or walls. These are slow as we have to copy data
		/// piecemeal to allow those effects to take place instead of copying the entire column as a
		/// memory span
		/// </summary>
		public Dictionary<int, bool> slowColumns = [];

		/// <summary>
		/// The actual tile data entries. Each entry represents an ITileData sequence. Vanilla by default
		/// has 5 of these, and mods may add more.
		/// </summary>
		public Dictionary<string, ITileDataEntry> dataEntries = [];

		/// <summary>
		/// This represents the NBT component of the structure file, used to store information such as
		/// tile entity data and chest loot pools.
		/// </summary>
		public List<StructureNBTEntry> nbtData;

		/// <summary>
		/// Processes the tile and wall type data to replace the types with those that should correspond to them
		/// in the current game instance, since those IDs shift with every reload
		/// </summary>
		public void PreProcessModdedTypes()
		{
			// Tracks types for repopulating with valid mapping later
			HashSet<ushort> tileTypesToRepopulate = new();
			HashSet<ushort> wallTypesToRepopulate = new();

			ITileDataEntry tileTypes = dataEntries["Terraria/TileTypeData"];
			ITileDataEntry wallTypes = dataEntries["Terraria/WallTypeData"];

			var dataPtr = (TileTypeData*)tileTypes.GetRawPtr();
			var wallPtr = (WallTypeData*)wallTypes.GetRawPtr();

			for (int k = 0; k < width * height; k++)
			{
				ushort typ = dataPtr[k].Type;
				ushort walltyp = wallPtr[k].Type;

				if (moddedTileTable.ContainsKey(typ))
				{
					dataPtr[k].Type = moddedTileTable[typ];
					tileTypesToRepopulate.Add(moddedTileTable[typ]);
				}

				if (moddedWallTable.ContainsKey(walltyp))
				{
					wallPtr[k].Type = moddedWallTable[walltyp];
					wallTypesToRepopulate.Add(moddedWallTable[walltyp]);
				}
			}

			// We repopulate the table with the updated types so that this data can be re-serialized
			// and remain valid if anyone ever needs to do that
			moddedTileTable.Clear();
			for(int k = 0; k < tileTypesToRepopulate.Count; k++)
			{
				moddedTileTable.Add(tileTypesToRepopulate.ElementAt(k), tileTypesToRepopulate.ElementAt(k));
			}

			moddedWallTable.Clear();
			for(int k = 0; k < wallTypesToRepopulate.Count; k++)
			{
				moddedWallTable.Add(wallTypesToRepopulate.ElementAt(k), wallTypesToRepopulate.ElementAt(k));
			}
		}

		/// <summary>
		/// Gets the current numeric ID of a tile given its fully qualified name
		/// </summary>
		/// <param name="entry">The fully qualified name of a modded tile</param>
		/// <returns>The current numeric ID of that tile, or 0 (dirt) if it does not exist</returns>
		public static ushort TileIDFromString(string entry)
		{
			string[] parts = entry.Split("/", 2);

			if (entry == "StructureHelper/Content/Tiles/NullBlock")
				return StructureHelper.NULL_IDENTIFIER;
			else if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModTile modTileType))
				return modTileType.Type;
			else
				return 0;
		}

		/// <summary>
		/// Gets the current numeric ID of a wall given its fully qualified name
		/// </summary>
		/// <param name="entry">The fully qualified name of a modded wall</param>
		/// <returns>The current numeric ID of that wall, or 0 (empty) if it does not exist</returns>
		public static ushort WallIDFromString(string entry)
		{
			string[] parts = entry.Split("/", 2);

			if (entry == "StructureHelper/Content/Tiles/NullWall")
				return StructureHelper.NULL_IDENTIFIER;
			else if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModWall modWallType))
				return modWallType.Type;
			else
				return 0;
		}

		/// <summary>
		/// Attempts to get the Type of an ITileData given a string representing it
		/// </summary>
		/// <param name="entry">The string for the ITileData</param>
		/// <returns>The Type representing the ITileData, or null it not found</returns>
		public static Type ITileDataFromString(string entry)
		{
			string[] parts = entry.Split("/", 2);

			if (parts.Length != 2)
				return null;

			if (parts[0] == "Terraria")
			{
				return parts[1] switch
				{
					nameof(TileTypeData) => typeof(TileTypeData),
					nameof(WallTypeData) => typeof(WallTypeData),
					nameof(LiquidData) => typeof(LiquidData),
					nameof(TileWallBrightnessInvisibilityData) => typeof(TileWallBrightnessInvisibilityData),
					nameof(TileWallWireStateData) => typeof(TileWallWireStateData),
					_ => null
				};
			}
			else
			{
				if (!ModLoader.TryGetMod(parts[0], out Mod mod))
					return null;

				return AssemblyManager.GetLoadableTypes(mod.Code).FirstOrDefault(n => n.FullName == parts[1]);
			}
		}

		/// <summary>
		/// Constructs a StructureData from raw binary data
		/// </summary>
		/// <param name="reader">A reader for the raw binary data, such as from a file</param>
		/// <returns>A StructureData constructed from the raw bytes</returns>
		/// <exception cref="InvalidDataException">If the stream does not represent a structure</exception>
		public static StructureData FromStream(BinaryReader reader)
		{
			string headerText = reader.ReadString();

			if (headerText != HEADER_TEXT)
				throw new InvalidDataException(ErrorHelper.GenerateErrorMessage("Attempted to deserialize binary data that is not a 3.0 structure file! Did you pass the path to a .shstruct file? If so, did you change the file extension without actually porting your structure file from 2.0?", null));

			var data = new StructureData
			{
				// Read the header, containing the width, height, version, and if the data contains an NBT section or not
				width = reader.ReadInt32(),
				height = reader.ReadInt32(),
				containsNbt = reader.ReadBoolean(),
				version = Version.Parse(reader.ReadString())
			};

			// Read the tile type table
			int tileTableLength = reader.ReadInt32();
			for (int k = 0; k < tileTableLength; k++)
			{
				ushort localId = reader.ReadUInt16();
				string tileKey = reader.ReadString();
				data.moddedTileTable[localId] = TileIDFromString(tileKey);
			}

			// Read the wall type table
			int wallTableLength = reader.ReadInt32();
			for (int k = 0; k < wallTableLength; k++)
			{
				ushort localId = reader.ReadUInt16();
				string wallKey = reader.ReadString();
				data.moddedWallTable[localId] = WallIDFromString(wallKey);
			}

			// Read slow column table
			for(int k = 0; k < data.width; k++)
			{
				data.slowColumns.Add(k, reader.ReadBoolean());
			}

			// Read the data blocks
			int dataEntryCount = reader.ReadInt32();
			for (int k = 0; k < dataEntryCount; k++)
			{
				// get name of this block
				string blockName = reader.ReadString();

				// get length of this block
				int blockLength = reader.ReadInt32();

				Type dataType = ITileDataFromString(blockName);

				if (dataType is null)
				{
					reader.BaseStream.Position += blockLength;
					continue;
				}
				else
				{
					Type thisEntryType = typeof(TileDataEntry<>).MakeGenericType(dataType);
					data.dataEntries[blockName] = (ITileDataEntry)Activator.CreateInstance(thisEntryType, [data.width * data.height, data.height]);
					data.dataEntries[blockName].SetData(reader.ReadBytes(blockLength));
				}
			}

			// Read NBT if applicable
			if (data.containsNbt)
			{
				TagCompound tag = TagIO.Read(reader);
				data.nbtData = (List<StructureNBTEntry>)tag.GetList<StructureNBTEntry>("nbtEntries");
			}

			data.PreProcessModdedTypes();
			return data;
		}

		/// <summary>
		/// Internal function used to copy a column of tile data into this StructureData
		/// </summary>
		/// <typeparam name="T">The type of the ITileData to copy in</typeparam>
		/// <param name="x">The X position of the column</param>
		/// <param name="y">The topmost point of the column</param>
		/// <param name="colIdx">The column in the data map to insert the data into</param>
		/// <param name="mod">The mod the ITileData type is from, or null if vanilla</param>
		internal void ImportDataColumn<T>(int x, int y, int colIdx, string key) where T : unmanaged, ITileData
		{
			if (!dataEntries.ContainsKey(key))
				dataEntries.Add(key, new TileDataEntry<T>(width * height, height));

			fixed (void* ptr = &Main.tile[x, y].Get<T>())
			{
				dataEntries[key].ImportColumn(ptr, colIdx);
			}
		}

		/// <summary>
		/// Internal function used to copy a column of tile data into the world from this StructureData
		/// </summary>
		/// <typeparam name="T">The type of the ITileData to copy out</typeparam>
		/// <param name="x">The X position of the column</param>
		/// <param name="y">The topmost point of the column</param>
		/// <param name="colIdx">The column in the data map to insert the data into</param>
		/// <param name="mod">The mod the ITileData type is from, or null if vanilla</param>
		internal void ExportDataColumn<T>(int x, int y, int colIdx, string key) where T : unmanaged, ITileData
		{
			fixed (void* ptr = &Main.tile[x, y].Get<T>())
			{
				dataEntries[key].ExportColumn(ptr, colIdx);
			}
		}

		/// <summary>
		/// Exports all data columns, for use when custom ITileData is present so reflection is required
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="colIdx"></param>
		internal void ExportAllDataColumns(int x, int y, int colIdx)
		{
			foreach (var entry in dataEntries)
			{
				var dataType = entry.Value.GetDataType();
				var methodInfo = typeof(StructureData).GetMethod("ExportDataColumn").MakeGenericMethod(dataType);
				methodInfo.Invoke(this, [x, y, colIdx, entry.Key]);
			}
		}

		/// <summary>
		/// Internal function used to copy a column of tile data into the world from this StructureData
		/// that may have null blocks or walls in it
		/// </summary>
		/// <typeparam name="T">The type of the ITileData to copy out</typeparam>
		/// <param name="x">The X position of the column</param>
		/// <param name="y">The topmost point of the column</param>
		/// <param name="colIdx">The column in the data map to insert the data into</param>
		/// <param name="mod">The mod the ITileData type is from, or null if vanilla</param>
		internal void ExportDataColumnSlow<T>(int x, int y, int colIdx, string key) where T : unmanaged, ITileData
		{
			if (key != "Terraria/WallTypeData")
			{
				ITileDataEntry typeData = dataEntries["Terraria/TileTypeData"];

				for (int rowIdx = 0; rowIdx < height; rowIdx++)
				{
					bool isNull = (*(TileTypeData*)typeData.GetSingleEntry(colIdx, rowIdx)).Type == StructureHelper.NULL_IDENTIFIER;

					if (!isNull)
					{
						fixed (void* ptr = &Main.tile[x, y + rowIdx].Get<T>())
							dataEntries[key].ExportSingle(ptr, colIdx, rowIdx);
					}
				}
			}
			else
			{
				ITileDataEntry typeData = dataEntries["Terraria/WallTypeData"];

				for (int rowIdx = 0; rowIdx < height; rowIdx++)
				{
					bool isNull = (*(WallTypeData*)typeData.GetSingleEntry(colIdx, rowIdx)).Type == StructureHelper.NULL_IDENTIFIER;

					if (!isNull)
					{
						fixed (void* ptr = &Main.tile[x, y + rowIdx].Get<T>())
							dataEntries[key].ExportSingle(ptr, colIdx, rowIdx);
					}
				}
			}

			return;
		}

		/// <summary>
		/// Exports all data columns that may have null blocks or walls in it, for use when custom ITileData is present so reflection is required
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="colIdx"></param>
		internal void ExportAllDataColumnsSlow(int x, int y, int colIdx)
		{
			foreach (var entry in dataEntries)
			{
				var dataType = entry.Value.GetDataType();
				var methodInfo = typeof(StructureData).GetMethod("ExportDataColumnSlow").MakeGenericMethod(dataType);
				methodInfo.Invoke(this, [x, y, colIdx, entry.Key]);
			}
		}

		/// <summary>
		/// Constructs a structure data from a region in the world
		/// </summary>
		/// <param name="x">The leftmost point of the region</param>
		/// <param name="y">The topmost point of the region</param>
		/// <param name="w">The width of the region</param>
		/// <param name="h">The height of the region</param>
		/// <returns>A StructureData representing the specified world region</returns>
		public static StructureData FromWorld(int x, int y, int w, int h)
		{
			var data = new StructureData
			{
				width = w,
				height = h,
				version = StructureHelper.Instance.Version
			};
			// Has NBT will be determined after scanning for NBT data

			for (int scanX = 0; scanX < w; scanX++)
			{
				data.ImportDataColumn<TileTypeData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<WallTypeData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<LiquidData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<TileWallBrightnessInvisibilityData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<TileWallWireStateData>(x + scanX, y, scanX, null);

				data.slowColumns.Add(scanX, false);

				for (int scanY = 0; scanY < h; scanY++)
				{
					var point = new Point16(x + scanX, y + scanY);
					Tile tile = Main.tile[point];
					ushort tileType = tile.TileType;
					ushort wallType = tile.WallType;

					if (tileType > TileID.Count)
						data.moddedTileTable[tileType] = tileType;

					if (wallType > WallID.Count)
						data.moddedWallTable[wallType] = wallType;

					if (tileType == StructureHelper.NullTileID || wallType == StructureHelper.NullWallID)
						data.slowColumns[scanX] = true;

					bool isCustomChest = false; // Marks a custom chest TE is here and to not save static chest data

					// Save tile enttiy as NBT data
					if (TileEntity.ByPosition.TryGetValue(point, out TileEntity entity))
					{
						var modTileEntity = entity as ModTileEntity;
						string teName;

						if (modTileEntity != null)
							teName = modTileEntity.FullName;
						else
							teName = entity.type.ToString();
						
						if (!string.IsNullOrEmpty(teName))
						{
							data.nbtData ??= [];
							data.containsNbt = true;

							TagCompound tag = [];
							entity.SaveData(tag);
							data.nbtData.Add(new TileEntityNBTEntry(scanX, scanY, teName, tag));

							if (entity is ChestEntity)
								isCustomChest = true;
						}
					}

					// Save sign as NBT data
					if (Main.tileSign[tile.TileType] && Main.sign.Any(n => n != null && n.x == point.X && n.y == point.Y))
					{
						int signIdx = Sign.ReadSign(point.X, point.Y, false);
						if (signIdx != -1)
						{
							data.nbtData ??= [];
							data.containsNbt = true;

							data.nbtData.Add(new SignNBTEntry(scanX, scanY, Main.sign[signIdx].text));
						}
					}

					// Save potential static chest contents
					if (!isCustomChest && Main.tileContainer[tile.TileType] && Main.chest.Any(n => n != null && n.x == point.X && n.y == point.Y))
					{
						int chestIdx = Chest.FindChest(point.X, point.Y);
						if (chestIdx != -1)
						{
							data.nbtData ??= [];
							data.containsNbt = true;

							data.nbtData.Add(new StaticChestNBTEntry(scanX, scanY, Main.chest[chestIdx].item));
						}
					}
				}
			}

			// We specifically change the markers for nulls to be ushort.MaxValue so they can be detected regardless
			// of if StructureHelper is loaded when generating
			if (StructureHelper.NullTileID != default)
			{
				if (data.moddedTileTable.ContainsKey(StructureHelper.NullTileID))
					data.moddedTileTable[StructureHelper.NullTileID] = StructureHelper.NULL_IDENTIFIER;

				if (data.moddedWallTable.ContainsKey(StructureHelper.NullWallID))
					data.moddedWallTable[StructureHelper.NullWallID] = StructureHelper.NULL_IDENTIFIER;
			}

			return data;
		}

		/// <summary>
		/// Adds custom ITileData from the region of the world, to be called after FromWorld initializes a StructureData
		/// </summary>
		/// <param name="x">The leftmost point of the region</param>
		/// <param name="y">The topmost point of the region</param>
		/// <param name="w">The width of the region</param>
		/// <param name="h">The height of the region</param>
		/// <param name="toSave">A list of custom ITileData types to save</param>
		public void AddCustomDataFromWorld(int x, int y, int w, int h, List<Type> toSave)
		{
			foreach (Type type in toSave)
			{
				if (type.IsAssignableFrom(typeof(ITileData)))
					typeof(StructureData).GetMethod("ImportDataColumn").MakeGenericMethod(type).Invoke(this, [x, y, w, h]);
				else
					throw new ArgumentException("All types passed to AddCustomDataFromWorld must implement ITileData");
			}

			containsCustomTileData = true;
		}

		/// <summary>
		/// Serialize this StructureData into a binary writer, such as to write to a file
		/// </summary>
		public void Serialize(BinaryWriter writer)
		{
			// Write the header
			writer.Write(HEADER_TEXT);
			writer.Write(width);
			writer.Write(height);
			writer.Write(containsNbt);
			writer.Write(version.ToString());

			// Write tile table
			writer.Write(moddedTileTable.Count);
			for (int k = 0; k < moddedTileTable.Count; k++)
			{
				KeyValuePair<ushort, ushort> pair = moddedTileTable.ElementAt(k);
				writer.Write(pair.Key);

				if (pair.Value == StructureHelper.NULL_IDENTIFIER)
					writer.Write("StructureHelper/Content/Tiles/NullBlock");
				else
					writer.Write(ModContent.GetModTile(pair.Value).FullName);
			}

			// Write wall table
			writer.Write(moddedWallTable.Count);
			for (int k = 0; k < moddedWallTable.Count; k++)
			{
				KeyValuePair<ushort, ushort> pair = moddedWallTable.ElementAt(k);
				writer.Write(pair.Key);

				if (pair.Value == StructureHelper.NULL_IDENTIFIER)
					writer.Write("StructureHelper/Content/Tiles/NullWall");
				else
					writer.Write(ModContent.GetModWall(pair.Value).FullName);
			}

			// Write slow column table
			for(int k = 0; k < width; k++)
			{
				writer.Write(slowColumns[k]);
			}

			// Write data blocks
			writer.Write(dataEntries.Count);
			for (int k = 0; k < dataEntries.Count; k++)
			{
				KeyValuePair<string, ITileDataEntry> pair = dataEntries.ElementAt(k);
				writer.Write(pair.Key);
				writer.Write(pair.Value.GetRawSize());
				writer.Write(pair.Value.GetData());
			}

			// Write NBT if applicable
			if (containsNbt && nbtData != null && nbtData.Count > 0)
			{
				TagIO.Write(new() {["nbtEntries"] = nbtData}, writer);
			}
		}
	}
}
