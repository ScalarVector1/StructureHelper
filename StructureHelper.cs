global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ModLoader;
using Terraria.ModLoader;

namespace StructureHelper
{
	public class StructureHelper : Mod
	{
		public StructureHelper() { Instance = this; }

		public static StructureHelper Instance { get; set; }

		public override void Unload()
		{
			Generator.StructureDataCache.Clear();
		}
	}
}