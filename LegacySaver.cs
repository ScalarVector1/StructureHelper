using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
	/// <summary>
	/// A static class providing utilities for saving structures.
	/// </summary>
	public static class Saver
	{
		/// <summary>
		/// Saves a given region of the world as a structure file
		/// </summary>
		/// <param name="target">The region of the world to save, in tile coordinates</param>
		/// <param name="targetPath">The name of the file to save. Automatically defaults to a file named after the date in the SavedStructures folder</param>
		public static void SaveToFile(Rectangle target, string targetPath = null, string name = "unnamed structure")
		{
			if (name == "")
				name = "unnamed structure";

			string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string thisPath = targetPath ?? Path.Combine(path, name);

			int counter = 2;
			while (File.Exists(thisPath))
			{
				thisPath = targetPath ?? Path.Combine(path, name) + $"({counter})";
				counter++;
			}

			Main.NewText("Structure saved as " + thisPath, Color.Yellow);
			FileStream stream = File.Create(thisPath);
			stream.Close();

			TagCompound tag = SaveStructure(target);
			TagIO.ToFile(tag, thisPath);
		}

		/// <summary>
		/// Saves a given list of TagCompounds together as a multistructure file
		/// </summary>
		/// <param name="toSave">The tags to save</param>
		/// <param name="targetPath">The name of the file to save. Automatically defaults to a file named after the date in the SavedStructures folder</param>
		public static void SaveMultistructureToFile(ref List<TagCompound> toSave, string targetPath = null, string name = "unnamed multistructure")
		{
			string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string thisPath = targetPath ?? Path.Combine(path, name);

			int counter = 2;
			while (File.Exists(thisPath))
			{
				thisPath = targetPath ?? Path.Combine(path, name) + $"({counter})";
				counter++;
			}

			Main.NewText("Structure saved as " + thisPath, Color.Yellow);
			FileStream stream = File.Create(thisPath);
			stream.Close();

			var tag = new TagCompound
			{
				{ "Structures", toSave },
				{ "Version", StructureHelper.Instance.Version.ToString() }
			};

			TagIO.ToFile(tag, thisPath);

			toSave.Clear();
		}

		/// <summary>
		/// Transforms a region of the world into a structure TagCompound. Must be called in an unsafe context!
		/// </summary>
		/// <param name="target">The region to transform</param>
		/// <returns>The TagCompound that can be saved to a structure file</returns>
		public unsafe static TagCompound SaveStructure(Rectangle target)
		{
			StructureHelper instance = StructureHelper.Instance;

			var tag = new TagCompound
			{
				{ "Version", instance?.Version.ToString() ?? "Unknown" },
				{ "Width", target.Width },
				{ "Height", target.Height }
			};

			var data = new List<LegacyTileSaveData>();
			for (int x = target.X; x <= target.X + target.Width; x++)
			{
				for (int y = target.Y; y <= target.Y + target.Height; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					string tileName;
					string wallName;
					string teName;

					if (tile.TileType >= TileID.Count)
						tileName = ModContent.GetModTile(tile.TileType).Mod.Name + " " + ModContent.GetModTile(tile.TileType).Name;
					else
						tileName = tile.TileType.ToString();

					if (tile.WallType >= WallID.Count)
						wallName = ModContent.GetModWall(tile.WallType).Mod.Name + " " + ModContent.GetModWall(tile.WallType).Name;
					else
						wallName = tile.WallType.ToString();

					TileEntity teTarget = null; //grabbing TE data
					var entityTag = new TagCompound();

					if (TileEntity.ByPosition.ContainsKey(new Point16(x, y)))
						teTarget = TileEntity.ByPosition[new Point16(x, y)];

					if (teTarget != null)
					{
						var modTileEntity = teTarget as ModTileEntity;

						if (modTileEntity != null)
						{
							teName = modTileEntity.Mod.Name + " " + modTileEntity.Name;
						}
						else
						{
							teName = teTarget.type.ToString();
						}

						teTarget.SaveData(entityTag);
					}
					else
					{
						teName = "";
					}

					int wallWireData;
					short packedLiquidData;
					byte brightInvisibleData;

					fixed (void* ptr = &tile.Get<TileWallWireStateData>())
					{
						int* intPtr = (int*)ptr;
						intPtr++;

						wallWireData = *intPtr;
					}

					fixed (void* ptr = &tile.Get<LiquidData>())
					{
						short* shortPtr = (short*)ptr;

						packedLiquidData = *shortPtr;
					}

					fixed (void* ptr = &tile.Get<TileWallBrightnessInvisibilityData>())
					{
						byte* bytePtr = (byte*)ptr;

						brightInvisibleData = *bytePtr;
					}

					data.Add(
						new LegacyTileSaveData(
							tileName,
							wallName,
							tile.TileFrameX,
							tile.TileFrameY,
							wallWireData,
							packedLiquidData,
							brightInvisibleData,
							teName,
							entityTag
							));
				}
			}

			tag.Add("TileData", data);
			return tag;
		}
	}
}