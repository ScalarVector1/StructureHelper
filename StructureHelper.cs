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