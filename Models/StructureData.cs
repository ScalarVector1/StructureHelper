﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ID;
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
			ITileDataEntry tileTypes = dataEntries["Terraria/TileTypeData"];
			ITileDataEntry wallTypes = dataEntries["Terraria/WallTypeData"];

			var dataPtr = (TileTypeData*)tileTypes.GetRawPtr();
			var wallPtr = (WallTypeData*)wallTypes.GetRawPtr();

			for (int k = 0; k < width * height; k++)
			{
				ushort typ = dataPtr[k].Type;
				ushort walltyp = wallPtr[k].Type;

				if (moddedTileTable.ContainsKey(typ))
					dataPtr[k].Type = moddedTileTable[typ];

				if (moddedWallTable.ContainsKey(walltyp))
					wallPtr[k].Type = moddedWallTable[walltyp];
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

			if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModTile modTileType))
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

			if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModWall modWallType))
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
		/// <returns>A StructureData constructed from the raw bytes, or null if the data is invalid or corrupted</returns>
		public static StructureData FromStream(BinaryReader reader)
		{
			string headerText = reader.ReadString();

			if (headerText != HEADER_TEXT)
				throw new InvalidDataException("Attempted to deserialize binary data that is not a structure file!");

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
				data.nbtData = [];
				TagCompound tag = TagIO.FromStream(reader.BaseStream, false);
				IList<TagCompound> nbtEntries = tag.GetList<TagCompound>("nbtEntries");

				for (int k = 0; k < nbtEntries.Count; k++)
				{
					StructureNBTEntry entry = new();
					entry.Deserialze(nbtEntries[k]);
					data.nbtData.Add(entry);
				}
			}

			data.PreProcessModdedTypes();
			return data;
		}

		public void ImportDataColumn<T>(int x, int y, int colIdx, Mod mod) where T : unmanaged, ITileData
		{
			string key = $"{mod?.Name ?? "Terraria"}/{typeof(T).Name}";

			if (!dataEntries.ContainsKey(key))
				dataEntries.Add(key, new TileDataEntry<T>(width * height, height));

			fixed (void* ptr = &Main.tile[x, y].Get<T>())
			{
				dataEntries[key].ImportColumn(ptr, colIdx);
			}
		}

		public void ExportDataColumn<T>(int x, int y, int colIdx, Mod mod) where T : unmanaged, ITileData
		{
			string key = $"{mod?.Name ?? "Terraria"}/{typeof(T).Name}";

			/*for(int k = 0; k < width; k++)
			{
				int thisX = x + k;
				fixed (void* ptr = &Main.tile[thisX, y].Get<T>())
					dataEntries[key].ExportSingle(ptr, rowIdx, k);
			}
			return;*/
			fixed (void* ptr = &Main.tile[x, y].Get<T>())
			{
				dataEntries[key].ExportColumn(ptr, colIdx);
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
			var data = new StructureData();
			data.width = w;
			data.height = h;
			data.version = StructureHelper.Instance.Version;
			// Has NBT will be determined after scanning for NBT data

			for(int scanX = 0; scanX < w; scanX++)
			{
				data.ImportDataColumn<TileTypeData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<WallTypeData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<LiquidData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<TileWallBrightnessInvisibilityData>(x + scanX, y, scanX, null);
				data.ImportDataColumn<TileWallWireStateData>(x + scanX, y, scanX, null);

				for (int scanY = 0; scanY < h; scanY++)
				{
					var point = new Point16(x + scanX, y + scanY);
					var tile = Main.tile[point];
					ushort tileType = tile.TileType;
					ushort wallType = tile.WallType;


					if (tileType > TileID.Count)
						data.moddedTileTable[tileType] = tileType;

					if (wallType > WallID.Count)
						data.moddedWallTable[wallType] = wallType;


					if (TileEntity.ByPosition.TryGetValue(point, out TileEntity entity))
					{
						data.nbtData ??= new();
						data.containsNbt = true;

						TagCompound tag = new();
						entity.SaveData(tag);
						data.nbtData.Add(new(point.X, point.Y, tag));
					}
				}
			}

			return data;
		}

		/// <summary>
		/// Serialize this StructureData into a binary writer, such as to write to a file
		/// </summary>
		/// <returns>A byte array that can later be deserialized into an identical StructureData</returns>
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
				writer.Write(ModContent.GetModTile(pair.Value).FullName);
			}

			// Write wall table
			writer.Write(moddedWallTable.Count);
			for (int k = 0; k < moddedWallTable.Count; k++)
			{
				KeyValuePair<ushort, ushort> pair = moddedWallTable.ElementAt(k);
				writer.Write(pair.Key);
				writer.Write(ModContent.GetModWall(pair.Value).FullName);
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
				List<TagCompound> tags = [];

				for (int k = 0; k < nbtData.Count; k++)
				{
					tags.Add(nbtData[k].Serialize());
				}

				TagIO.WriteTag("nbtEntries", tags, writer);
			}
		}
	}
}
