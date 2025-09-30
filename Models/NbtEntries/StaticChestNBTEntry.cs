using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace StructureHelper.Models.NbtEntries
{
	internal class StaticChestNBTEntry : StructureNBTEntry
	{
		public Item[] contents;

		public static Func<TagCompound, StaticChestNBTEntry> DESERIALIZER = Deserialize;

		public StaticChestNBTEntry(int x, int y, Item[] contents) : base(x, y)
		{
			this.contents = new Item[contents.Length];

			for (int k = 0; k < contents.Length; k++)
			{
				this.contents[k] = contents[k].Clone();
			}
		}

		public static StaticChestNBTEntry Deserialize(TagCompound tag)
		{
			StaticChestNBTEntry entry = new
				(tag.GetInt("x"),
				tag.GetInt("y"),
				tag.GetList<Item>("contents").ToArray());

			return entry;
		}

		public override void OnGenerate(Point16 generatingAt, bool ignoreNull, GenFlags flags)
		{
			int i = Chest.CreateChest(generatingAt.X, generatingAt.Y);

			if (i == -1)
				return;

			Chest chest = Main.chest[i];

			for (int k = 0; k < contents.Length; k++)
			{
				chest.item[k] = contents[k].Clone();
			}
		}

		public override void Serialize(TagCompound tag)
		{
			tag["contents"] = contents.ToList();
		}
	}
}