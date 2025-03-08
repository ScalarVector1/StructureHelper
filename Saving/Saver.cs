using StructureHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.Saving
{
	internal class Saver
	{
		public static void SaveToFile(StructureData data, string targetPath = null, string name = "unnamed structure")
		{
			if (String.IsNullOrEmpty(name))
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

			thisPath += ".shstruct";

			Main.NewText("Structure saved as " + thisPath, Color.Yellow);


			using FileStream fileStream = File.Create(thisPath);
			using GZipStream compressionStream = new GZipStream(fileStream, CompressionLevel.Optimal);
			using BinaryWriter writer = new BinaryWriter(compressionStream);
			data.Serialize(writer);
		}
	}
}
