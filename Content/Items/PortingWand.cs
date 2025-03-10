using StructureHelper.NewFolder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace StructureHelper.Content.Items
{
	internal class PortingWand : ModItem
	{
		public override string Texture => "StructureHelper/Assets/Items/" + Name;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Porting Wand");
			Tooltip.SetDefault("Automatically ports every structure in your SavedStructures directory.");
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = ItemRarityID.Blue;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override bool? UseItem(Player player)
		{
			string folderPath = ModLoader.ModPath.Replace("Mods", "SavedStructures");
			Directory.CreateDirectory(folderPath);

			Porter.PortDirectory(folderPath);
			return true;
		}
	}
}
