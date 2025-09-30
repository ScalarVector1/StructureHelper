using StructureHelper.Helpers;
using StructureHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.API
{
	/// <summary>
	/// In this class you will find utilities for saving regions of the world into structure data,
	/// and then saving that data into files if needed.
	/// </summary>
	public class Saver
	{
		/// <summary>
		/// Saves a region of the world into a StructureData object.
		/// </summary>
		/// <param name="x">The leftmost point of the region</param>
		/// <param name="y">The topmost point of the region</param>
		/// <param name="width">The width of the region</param>
		/// <param name="height">The height of the region</param>
		/// <returns>a StructureData object representing that region of the world</returns>
		/// <exception cref="ArgumentException">If any part of the region is outside of the world</exception>
		public static StructureData SaveToStructureData(int x, int y, int width, int height)
		{
			if (x < 0 || x + width > Main.maxTilesX || y < 0 || y + height > Main.maxTilesY)
				throw new ArgumentException(ErrorHelper.GenerateErrorMessage("Attempted to save structure data for a region outside of the world.", null));

			return StructureData.FromWorld(x, y, width, height);
		}

		/// <summary>
		/// Saves a region of the world into a StructureData object, including the specified custom ITileData types.
		/// </summary>
		/// <param name="x">The leftmost point of the region</param>
		/// <param name="y">The topmost point of the region</param>
		/// <param name="width">The width of the region</param>
		/// <param name="height">The height of the region</param>
		/// <param name="customTypes">The types of ITileData to save in addition to vanilla ITileData. These should all be blittable ITileData types.</param>
		/// <returns>a StructureData object representing that region of the world</returns>
		/// <exception cref="ArgumentException">If any part of the region is outside of the world</exception>
		public static StructureData SaveToStructureDataWithCustom(int x, int y, int width, int height, List<Type> customTypes)
		{
			var data = SaveToStructureData(x, y, width, height);
			data.AddCustomDataFromWorld(x, y, width, height, customTypes);

			return data;
		}

		/// <summary>
		/// Saves a given StructureData to a file, the location is by the target path, and with the given name.
		/// </summary>
		/// <param name="data">The StructureData to save</param>
		/// <param name="targetPath">The path to the directory save the file into</param>
		/// <param name="name">The name of the file, NOT including the extension</param>
		public static void SaveToFile(StructureData data, string targetPath = null, string name = "unnamed structure")
		{
			if (string.IsNullOrEmpty(name))
				name = "unnamed structure";

			string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string thisPath = targetPath ?? Path.Combine(path, name);

			int counter = 2;
			while (File.Exists(thisPath + ".shstruct"))
			{
				thisPath = (targetPath ?? Path.Combine(path, name)) + $"({counter})";
				counter++;
			}

			thisPath += ".shstruct";

			using FileStream fileStream = File.Create(thisPath);
			using var compressionStream = new GZipStream(fileStream, CompressionLevel.Optimal);
			using var writer = new BinaryWriter(compressionStream);
			data.Serialize(writer);

			Main.NewText("Structure saved as " + thisPath, Color.Yellow);
		}

		/// <summary>
		/// Saves a given MultiStructureData to a file, the location is by the target path, and with the given name.
		/// </summary>
		/// <param name="data">The MultiStructureData to save</param>
		/// <param name="targetPath">The path to the directory save the file into</param>
		/// <param name="name">The name of the file, NOT including the extension</param>
		public static void SaveMultistructureToFile(MultiStructureData data, string targetPath = null, string name = "unnamed multistructure")
		{
			if (string.IsNullOrEmpty(name))
				name = "unnamed structure";

			string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string thisPath = targetPath ?? Path.Combine(path, name);

			int counter = 2;
			while (File.Exists(thisPath + ".shmstruct"))
			{
				thisPath = (targetPath ?? Path.Combine(path, name)) + $"({counter})";
				counter++;
			}

			thisPath += ".shmstruct";

			using FileStream fileStream = File.Create(thisPath);
			using var compressionStream = new GZipStream(fileStream, CompressionLevel.Optimal);
			using var writer = new BinaryWriter(compressionStream);
			data.Serialize(writer);

			Main.NewText("MultiStructure saved as " + thisPath, Color.Yellow);
		}
	}
}
