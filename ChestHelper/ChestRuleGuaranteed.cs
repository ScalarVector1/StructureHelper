﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.IO;

namespace StructureHelper.ChestHelper
{
    class ChestRuleGuaranteed : ChestRule
    {
        public override string Name => "Guaranteed Rule";

        public override string Tooltip => "Always generates every item in the rule\nItems are generated in the order they appear here";

        public override void PlaceItems(Chest chest, ref int nextIndex)
        {
            if (nextIndex >= 40) return;

            for (int k = 0; k < pool.Count; k++)
            {
                if (nextIndex >= 40) return;
                chest.item[nextIndex] = pool[k].GetLoot();
                nextIndex++;
            }
        }

        public override TagCompound Serizlize()
        {
            var tag = new TagCompound()
            {
                {"Type", "Guaranteed"},
                {"Pool", SerializePool()}
            };

            return tag;
        }

        public static ChestRule Deserialize(TagCompound tag)
        {
            var rule = new ChestRuleGuaranteed();
            rule.pool = DeserializePool(tag.GetCompound("Pool"));

            return rule;
        }
    }
}
