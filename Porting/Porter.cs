using StructureHelper.API;
using StructureHelper.Content.GUI;
using StructureHelper.Models;
using StructureHelper.Models.NbtEntries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static System.Net.WebRequestMethods;

namespace StructureHelper.NewFolder
{
	internal class Porter
	{
		public static void AddDataEntry<T>(StructureData target, Mod mod) where T:unmanaged, ITileData
		{
			string key = $"{mod?.Name ?? "Terraria"}/{typeof(T).Name}";

			if (!target.dataEntries.ContainsKey(key))
				target.dataEntries.Add(key, new TileDataEntry<T>(target.width * target.height, target.height));
		}

		public static unsafe void InsertDataEntry<T>(StructureData target, Mod mod, int x, int y, void* value) where T : unmanaged, ITileData
		{
			string key = $"{mod?.Name ?? "Terraria"}/{typeof(T).Name}";

			void* memTarget = target.dataEntries[key].GetSingleEntry(x, y);
			Buffer.MemoryCopy(value, memTarget, sizeof(T), sizeof(T));		
		}

		public static unsafe StructureData PortStructure(TagCompound tag)
		{
			StructureData ported = new StructureData();

			var data = (List<TileSaveData>)tag.GetList<TileSaveData>("TileData");

			int width = tag.GetInt("Width");
			int height = tag.GetInt("Height");

			ported.width = width + 1;
			ported.height = height + 1;
			ported.version = StructureHelper.Instance.Version;

			AddDataEntry<TileTypeData>(ported, null);
			AddDataEntry<WallTypeData>(ported, null);
			AddDataEntry<LiquidData>(ported, null);
			AddDataEntry<TileWallBrightnessInvisibilityData>(ported, null);
			AddDataEntry<TileWallWireStateData>(ported, null);

			for (int x = 0; x <= width; x++)			
			{
				bool isSlow = false;

				for (int y = 0; y <= height; y++)
				{
					int index = y + x * (height + 1);
					TileSaveData d = data[index];

					bool isNullTile = false;
					bool isNullWall = false;

					if (!ushort.TryParse(d.tile, out ushort type))
					{
						string[] parts = d.tile.Split();

						if (parts[0] == "StructureHelper" && parts[1] == "NullBlock")
						{
							ported.moddedTileTable.TryAdd(StructureHelper.NullTileID, StructureHelper.NULL_IDENTIFIER);
							type = StructureHelper.NULL_IDENTIFIER;
							isSlow = true;
						}
						else if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind<ModTile>(parts[1], out ModTile modTileType))
						{
							ported.moddedTileTable.TryAdd(modTileType.Type, modTileType.Type);
							type = modTileType.Type;
						}
						else
						{
							type = 0;
						}
					}

					if (!ushort.TryParse(d.wall, out ushort wallType))
					{
						string[] parts = d.wall.Split();

						if (parts[0] == "StructureHelper" && parts[1] == "NullWall")
						{
							ported.moddedWallTable.TryAdd(StructureHelper.NullWallID, StructureHelper.NULL_IDENTIFIER);
							wallType = StructureHelper.NULL_IDENTIFIER;
							isSlow = true;
						}
						else if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind<ModWall>(parts[1], out ModWall modWallType))
						{
							ported.moddedWallTable.TryAdd(modWallType.Type, modWallType.Type);
							wallType = modWallType.Type;
						}
						else
						{
							wallType = 0;
						}
					}

					if (!d.Active)
						isNullTile = false;

					InsertDataEntry<TileTypeData>(ported, null, x, y, &type);
					InsertDataEntry<WallTypeData>(ported, null, x, y, &wallType);
					InsertDataEntry<LiquidData>(ported, null, x, y, &d.packedLiquidData);
					InsertDataEntry<TileWallBrightnessInvisibilityData>(ported, null, x, y, &d.brightInvisibleData);

					var reconstructed = new TileWallWireStateData();
					reconstructed.TileFrameX = d.frameX;
					reconstructed.TileFrameY = d.frameY;

					void* ptr = &reconstructed;

					int* intPtr = (int*)ptr;
					intPtr++;

					*intPtr = d.wallWireData;

					if (!d.Active)
						reconstructed.HasTile = false;

					InsertDataEntry<TileWallWireStateData>(ported, null, x, y, &reconstructed);


					if (d.TEType != "") //place and load a tile entity
					{

						if (!int.TryParse(d.TEType, out int typ))
						{
							string[] parts = d.TEType.Split();

							if (ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind<ModTileEntity>(parts[1], out ModTileEntity te))
								typ = te.Type;
						}

						if (typ != 0)
						{
							var entity = TileEntity.manager.GenerateInstance(typ);

							var modTileEntity = entity as ModTileEntity;
							string teName;

							if (modTileEntity != null)
								teName = modTileEntity.FullName;
							else
								teName = entity.type.ToString();

							ported.containsNbt = true;
							ported.nbtData ??= new();

							ported.nbtData.Add(new TileEntityNBTEntry(x, y, teName, d.TEData));
						}
					}
				}
				ported.slowColumns.Add(x, isSlow);
			}

			return ported;
		}

		public static MultiStructureData PortMultiStructure(TagCompound tag)
		{
			MultiStructureData ported = new MultiStructureData();

			var oldStructures = (List<TagCompound>)tag.GetList<TagCompound>("Structures");

			ported.count = oldStructures.Count;

			for(int k = 0; k < oldStructures.Count; k++)
			{
				ported.structures.Add(PortStructure(oldStructures[k]));
			}

			return ported;
		}

		public static void PortFile(string path)
		{
			var tag = API.Legacy.LegacyGenerator.GetTag(path, StructureHelper.Instance, true);
			var data = PortStructure(tag);

			Saver.SaveToFile(data, path);
		}

		public static void PortMultiFile(string path)
		{
			var tag = API.Legacy.LegacyGenerator.GetTag(path, StructureHelper.Instance, true);
			var data = PortMultiStructure(tag);

			Saver.SaveMultistructureToFile(data, path);
		}

		public static void PortDirectory(string dir)
		{
			string[] filePaths = Directory.GetFiles(dir);

			foreach (string path in filePaths)
			{
				string name = Path.GetFileName(path);

				if (!Path.HasExtension(path))
				{
					try
					{
						if (API.Legacy.LegacyGenerator.GetTag(path, StructureHelper.Instance, true).ContainsKey("Structures"))
							PortMultiFile(path);
						else
							PortFile(path);
					}
					catch
					{
						Main.NewText($"A file in your SavedStructures directory didnt seem to be a structure ({name}), it will be skipped!");
						continue;
					}
				}
			}
		}
	}
}
