using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.IO;

namespace StructureHelper.ChestHelper
{
    class ChestRule
    {
        public List<Loot> pool = new List<Loot>();

        public void AddItem(Item item)
        {
            pool.Add(new Loot(item.DeepClone(), 1));
        }

        public void RemoveItem(Loot loot) => pool.Remove(loot);

        public virtual void PlaceItems(Chest chest, ref int nextIndex) {}

        public virtual TagCompound Serizlize() => null;

        public static ChestRule Deserialize(TagCompound tag)
        {
            string str = tag.GetString("Type");

            ChestRule rule;

            switch (str)
            {
                case "Guaranteed": rule = ChestRuleGuaranteed.Deserialize(tag); break;
                case "Chance": rule = ChestRuleChance.Deserialize(tag); break;
                case "Pool": rule = ChestRulePool.Deserialize(tag); break;
                case "PoolChance": rule = ChestRulePoolChance.Deserialize(tag); break;
                default: rule = null; break;
            }

            return rule;
        }

        public TagCompound SerializePool()
        {
            TagCompound tag = new TagCompound();

            tag.Add("Count", pool.Count);

            for (int k = 0; k < pool.Count; k++)
                tag.Add("Pool" + k, pool[k].Serialize());

            return tag;
        }

        public static List<Loot> DeserializePool(TagCompound tag)
        {
            var loot = new List<Loot>();
            int count = tag.GetInt("Count");

            for(int k = 0; k < count; k++)
            {
                loot.Add(Loot.Deserialze(tag.GetCompound("Pool" + k)));
            }

            return loot;
        }
    }

    struct Loot
    {
        public Item LootItem;
        public int min;
        public int max;

        public Loot(Item item, int min, int max = 0)
        {
            this.min = min;
            this.max = max == 0 ? min : max;

            Item newItem = item.Clone();
            newItem.stack = 1;
            LootItem = newItem;
        }

        public Item GetLoot()
        {
            Item item = LootItem.Clone();
            LootItem.stack = WorldGen.genRand.Next(min, max);
            return item;
        }

        public TagCompound Serialize()
        {
            TagCompound tag = new TagCompound
            {
                { "Item", LootItem },
                { "Min", min },
                { "Max", max }
            };
            return tag;
        }

        public static Loot Deserialze(TagCompound tag)
        {
            return new Loot(tag.Get<Item>("Item"), tag.GetInt("Min"), tag.GetInt("Max"));
        }
    }
}
