using StructureHelper.API.Legacy;
using StructureHelper.Content.GUI;
using StructureHelper.Core;
using StructureHelper.Models;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace StructureHelper.Content.Items
{
	class MultistructureWand : StructureWand
	{
		internal List<StructureData> capturedData = [];

		public override string Texture => "StructureHelper/Assets/Items/" + Name;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Multistructure Wand");
			Tooltip.SetDefault("Select 2 points in the world, then right click to add a structure. Right click in your inventory when done to save.");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override ModItem Clone(Item newEntity)
		{
			var clone = base.Clone(newEntity) as MultistructureWand;
			clone.capturedData = [];
			return clone;
		}

		public override void RightClick(Player player)
		{
			Item.stack++;

			if (capturedData.Count > 1)
				NameConfirmPopup.OpenConfirmation((name) => API.Saver.SaveMultistructureToFile(MultiStructureData.FromStructureList(capturedData), name: name));
			else
				Main.NewText("Not enough structures! If you want to save a single structure, use the normal structure wand instead!", Color.Red);
		}

		public override void OnConfirmRectangle()
		{
			if (WandSavingSettings.activeCustomDataTypes.Count > 0)
				capturedData.Add(API.Saver.SaveToStructureDataWithCustom(TopLeft.X, TopLeft.Y, Width, Height, WandSavingSettings.activeCustomDataTypes));
			else
				capturedData.Add(API.Saver.SaveToStructureData(TopLeft.X, TopLeft.Y, Width, Height));

			Main.NewText("Structure captured! Total structures to save: " + capturedData.Count);
		}
	}
}