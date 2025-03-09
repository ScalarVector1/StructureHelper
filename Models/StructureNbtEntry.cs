using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace StructureHelper.Models
{
	/// <summary>
	/// Represents an NBT entry within the NBT section of the structure file. These are used for sparse,
	/// complex data within structures like tile entities or chest pools.
	/// 
	/// You MUST implement a DESERIALIZER as per TagSerializable!
	/// </summary>
	public abstract class StructureNBTEntry : TagSerializable
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
		/// What needs to happen on structure generation to properly place the object
		/// represented by this NBT entry into the world.
		/// </summary>
		public abstract void OnGenerate(Point16 generatingAt, bool ignoreNull, GenFlags flags);

		/// <summary>
		/// Serialize the custom data for this StructureNbtEntry here
		/// </summary>
		/// <param name="tag">The tag to serialize to</param>
		public abstract void Serialize(TagCompound tag);

		public StructureNBTEntry(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				["x"] = x,
				["y"] = y
			};

			Serialize(tag);

			return tag;
		}
	}
}
