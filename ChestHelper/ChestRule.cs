using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace StructureHelper.ChestHelper
{
	class ChestRule
	{
		public List<Loot> pool = new();

		public virtual bool UsesWeight => false;

		public virtual string Name => "Unknown Rule";

		public virtual string Tooltip => "Probably a bug! Report me!";

		public Loot AddItem(Item item)
		{
			var loot = new Loot(item.Clone(), 1);
			pool.Add(loot);

			return loot;
		}

		public void RemoveItem(Loot loot)
		{
			pool.Remove(loot);
		}

		public virtual void PlaceItems(Chest chest, ref int nextIndex) { }

		public virtual TagCompound Serizlize()
		{
			return null;
		}

		public static ChestRule Deserialize(TagCompound tag)
		{
			string str = tag.GetString("Type");

			ChestRule rule = str switch
			{
				"Guaranteed" => ChestRuleGuaranteed.Deserialize(tag),
				"Chance" => ChestRuleChance.Deserialize(tag),
				"Pool" => ChestRulePool.Deserialize(tag),
				"PoolChance" => ChestRulePoolChance.Deserialize(tag),
				_ => null,
			};

			return rule;
		}

		public TagCompound SerializePool()
		{
			var tag = new TagCompound
			{
				{ "Count", pool.Count }
			};

			for (int k = 0; k < pool.Count; k++)
			{
				tag.Add("Pool" + k, pool[k].Serialize());
			}

			return tag;
		}

		public static List<Loot> DeserializePool(TagCompound tag)
		{
			var loot = new List<Loot>();
			int count = tag.GetInt("Count");

			for (int k = 0; k < count; k++)
			{
				loot.Add(Loot.Deserialze(tag.GetCompound("Pool" + k)));
			}

			return loot;
		}

		public virtual ChestRule Clone()
		{
			var clone = new ChestRule();

			for (int k = 0; k < pool.Count; k++)
			{
				clone.pool.Add(pool[k].Clone());
			}

			return clone;
		}
	}

	class Loot
	{
		public Item givenItem;
		public int min;
		public int max;
		public int weight;

		public Loot(Item item, int min, int max = 0, int weight = 1)
		{
			this.min = min;
			this.max = max == 0 ? min : max;
			this.weight = weight;

			Item newItem = item.Clone();
			newItem.stack = 1;
			givenItem = newItem;

		}

		public Item GetLoot()
		{
			Item item = givenItem.Clone();
			item.stack = WorldGen.genRand.Next(min, max);
			return item;
		}

		public TagCompound Serialize()
		{
			var tag = new TagCompound
			{
				{ "Item", givenItem },
				{ "Min", min },
				{ "Max", max },
				{ "Weight", weight}
			};
			return tag;
		}

		public static Loot Deserialze(TagCompound tag)
		{
			return new Loot(
				tag.Get<Item>("Item"),
				tag.GetInt("Min"),
				tag.GetInt("Max"),
				tag.GetInt("Weight"));
		}

		public Loot Clone()
		{
			return new Loot(givenItem.Clone(), min, max, weight);
		}
	}
}
