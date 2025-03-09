using StructureHelper.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.Models
{
	/// <summary>
	/// Represents the contents of a multi structure file in-memory. This object is only valid
	/// for a given mod load instance, as modded tile types will be parsed out from the table
	/// into the body data. If you are storing these and your mod reloads, you should consider
	/// them invalid afterwards.
	/// </summary>
	public class MultiStructureData
	{
		public const string HEADER_TEXT = "STRUCTURE_HELPER_MULTI_STRUCTURE";

		/// <summary>
		/// The amount of structures in this multistructure
		/// </summary>
		public int count;

		/// <summary>
		/// The structure data in this multistructure
		/// </summary>
		public List<StructureData> structures = [];

		/// <summary>
		/// Constructs a MultiStructureData from raw binary data
		/// </summary>
		/// <param name="reader">A reader for the raw binary data, such as from a file</param>
		/// <returns>A MultiStructureData constructed from the raw bytes</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static MultiStructureData FromStream(BinaryReader reader)
		{
			string headerText = reader.ReadString();

			if (headerText != HEADER_TEXT)
				throw new InvalidDataException(ErrorHelper.GenerateErrorMessage("Attempted to deserialize binary data that is not a 3.0 multi structure file! Did you pass the path to a .shmstruct file? If so, did you change the file extension without actually porting your multi structure file from 2.0?", null));

			var data = new MultiStructureData();
			data.count = reader.ReadInt32();

			for(int k = 0; k < data.count; k++)
			{
				data.structures.Add(StructureData.FromStream(reader));
			}

			return data;
		}

		/// <summary>
		/// Generates a MultiStructureData from a list of StructureData
		/// </summary>
		/// <param name="source">The StructureData to put into this MultiStructureData</param>
		/// <returns>The constructed MultiStructureData containing all elements in the source list</returns>
		public static MultiStructureData FromStructureList(List<StructureData> source)
		{
			MultiStructureData data = new();
			data.count = source.Count;
			data.structures.AddRange(source);

			return data;
		}

		/// <summary>
		/// Serialize this MultiStructureData into a binary writer, such as to write to a file
		/// </summary>
		public void Serialize(BinaryWriter writer)
		{
			writer.Write(HEADER_TEXT);
			writer.Write(count);

			for(int k = 0; k < count; k++)
			{
				structures[k].Serialize(writer);
			}
		}
	}
}
