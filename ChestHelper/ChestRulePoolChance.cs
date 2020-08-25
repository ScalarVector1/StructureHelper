using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.IO;

namespace StructureHelper.ChestHelper
{
    class ChestRulePoolChance : ChestRule
    {
        /// <summary>
        /// How many items from the pool, picked at random, should be placed in the chest.
        /// </summary>
        public int itemsToGenerate;

        /// <summary>
        /// the chance for this item pool to generate at all.
        /// </summary>
        public float chance;

        public override void PlaceItems(Chest chest, ref int nextIndex)
        {
            if (nextIndex >= 40) return;

            if (WorldGen.genRand.NextFloat() <= chance)
            {
                List<Loot> toLoot = pool;
                Helper.RandomizeList<Loot>(ref toLoot);

                for (int k = 0; k < itemsToGenerate; k++)
                {
                    chest.item[nextIndex] = pool[k].GetLoot();
                    nextIndex++;
                }
            }
        }

        public override TagCompound Serizlize()
        {
            var tag = new TagCompound()
            {
                {"Type", "PoolChance"},
                {"Chance", chance},
                {"ToGenerate", itemsToGenerate},
                {"Pool", SerializePool()}
            };

            return tag;
        }

        public static ChestRule Deserialize(TagCompound tag)
        {
            var rule = new ChestRulePoolChance();
            rule.itemsToGenerate = tag.GetInt("ToGenerate");
            rule.chance = tag.GetFloat("Chance");
            rule.pool = DeserializePool(tag.GetCompound("Pool"));

            return rule;
        }
    }
}
