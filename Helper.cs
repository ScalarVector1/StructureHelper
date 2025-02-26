using System.Collections.Generic;
using System.Linq;

namespace StructureHelper
{
	static class Helper
	{
		/// <summary>
		/// Randomizes the order of a list using the Fischer-Yates algorithm
		/// </summary>
		/// <typeparam name="T">The type of the list to randomize</typeparam>
		/// <param name="input">The list to randomize</param>
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
	}
}