using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace StructureHelper
{
    internal static class Helper
    {
        /// <summary>
        /// Randomizes the order of a list using the Fischer-Yates algorithm
        /// </summary>
        /// <typeparam name="T">The type of the list to randomize</typeparam>
        /// <param name="input">The list to randomize</param>
        internal static void RandomizeList<T>(ref List<T> input)
        {
            int n = input.Count();
            while (n > 1)
            {
                n--;
                int k = WorldGen.genRand.Next(n + 1);
                T value = input[k];
                input[k] = input[n];
                input[n] = value;
            }
        }

        /// <summary>
        /// Gets the texture of an in-game item
        /// </summary>
        /// <param name="item">The item to retrieve the texture of</param>
        /// <returns>The texture of the passed item</returns>
        internal static Texture2D GetItemTexture(Item item)
        {
            if (item.type < Main.maxItemTypes) 
                return Terraria.GameContent.TextureAssets.Item[item.type].Value;
            else 
                return ModContent.Request<Texture2D>(item.ModItem.Texture).Value;
        }
    }
}
