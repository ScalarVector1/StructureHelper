using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace StructureHelper
{
	public static class Helper
	{
		//Fisher-Yates algorithm
		public static void RandomizeList<T>(ref List<T> input)
		{
			int n = input.Count();

			while (n > 1)
			{
				n--;
				int k = WorldGen.genRand.Next(n + 1);
				(input[n], input[k]) = (input[k], input[n]);
			}
		}

		public static Texture2D GetItemTexture(Item item)
		{
			if (item.type < Main.maxItemTypes)
				return Terraria.GameContent.TextureAssets.Item[item.type].Value;
			else
				return ModContent.Request<Texture2D>(item.ModItem.Texture).Value;
		}
	}
}
