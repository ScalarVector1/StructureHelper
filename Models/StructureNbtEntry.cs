using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace StructureHelper.Models
{
	/// <summary>
	/// Represents an NBT entry within the NBT section of the structure file. These are used for sparse,
	/// complex data within structures like tile entities or chest pools.
	/// </summary>
	public class StructureNBTEntry
	{
		/// <summary>
		/// X coordinate local to the strucure associated with this NBT entry
		/// </summary>
		public int x;

		/// <summary>
		/// Y coordinate local to the structure associated with this NBT entry
		/// </summary>
		public int y;

		/// <summary>
		/// The inner data contents of this NBT entry
		/// </summary>
		public TagCompound data;

		public StructureNBTEntry()
		{

		}

		public StructureNBTEntry(int x, int y, TagCompound data)
		{
			this.x = x;
			this.y = y;
			this.data = data;
		}

		public TagCompound Serialize()
		{
			TagCompound tag = new()
			{
				["x"] = x,
				["y"] = y,
				["data"] = data
			};

			return tag;
		}

		public void Deserialze(TagCompound tag)
		{
			x = tag.GetInt("x");
			y = tag.GetInt("y");
			data = tag.GetCompound("data");
		}
	}
}
