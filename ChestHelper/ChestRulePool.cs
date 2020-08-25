using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.IO;

namespace StructureHelper.ChestHelper
{
    class ChestRulePool : ChestRule
    {
        /// <summary>
        /// How many items from the pool, picked at random, should be placed in the chest.
        /// </summary>
        public int itemsToGenerate;

        public override void PlaceItems(Chest chest, ref int nextIndex)
        {
            if (nextIndex >= 40) return;

            List<Loot> toLoot = pool;
            Helper.RandomizeList<Loot>(ref toLoot);

            for(int k = 0; k < itemsToGenerate; k++)
            {
                chest.item[nextIndex] = pool[k].GetLoot();
                nextIndex++;
            }
        }

        public override TagCompound Serizlize()
        {
            var tag = new TagCompound()
            {
                {"Type", "Pool"},
                {"ToGenerate", itemsToGenerate},
                {"Pool", SerializePool()}
            };

            return tag;
        }

        public static ChestRule Deserialize(TagCompound tag)
        {
            var rule = new ChestRulePool();
            rule.itemsToGenerate = tag.GetInt("ToGenerate");
            rule.pool = DeserializePool(tag.GetCompound("Pool"));

            return rule;
        }
    }
}
