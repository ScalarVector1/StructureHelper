using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace StructureHelper.Models.NbtEntries
{
	internal class SignNBTEntry : StructureNBTEntry
	{
		public string text;

		public static Func<TagCompound, SignNBTEntry> DESERIALIZER = Deserialize;

		public SignNBTEntry(int x, int y, string text) : base(x, y) 
		{
			this.text = text;
		}

		public static SignNBTEntry Deserialize(TagCompound tag)
		{
			SignNBTEntry entry = new
				(tag.GetInt("x"), 
				tag.GetInt("y"),
				tag.GetString("text"));

			return entry;
		}

		public override void OnGenerate(Point16 generatingAt, bool ignoreNull, GenFlags flags)
		{
			int index = Sign.ReadSign(generatingAt.X, generatingAt.Y, true);

			// Index may be -1 if there are already 1000 signs in the world
			if (index != -1)
				Sign.TextSign(index, text);
		}

		public override void Serialize(TagCompound tag)
		{
			tag["text"] = text;
		}
	}
}
