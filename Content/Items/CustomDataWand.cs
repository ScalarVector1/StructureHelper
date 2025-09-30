using StructureHelper.Content.GUI;
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
	internal class CustomDataWand : ModItem
	{
		public override string Texture => "StructureHelper/Assets/Items/" + Name;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Custom Data Wand");
			Tooltip.SetDefault("Advanced tool. Allows toggling of custom tile data for structure saving after registration.\nSee documentation for more information.");
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
			TileDataConfiguratorMenu.OpenMenu();
			return true;
		}
	}
}
