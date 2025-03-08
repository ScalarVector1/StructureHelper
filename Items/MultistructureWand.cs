using StructureHelper.GUI;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace StructureHelper.Items
{
	class MultistructureWand : StructureWand
	{
		internal List<TagCompound> structureCache = [];

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
			clone.structureCache = [];
			return clone;
		}

		public override void RightClick(Player player)
		{
			Item.stack++;

			if (structureCache.Count > 1)
				NameConfirmPopup.OpenConfirmation((name) => LegacySaver.SaveMultistructureToFile(ref structureCache, name: name));
			else
				Main.NewText("Not enough structures! If you want to save a single structure, use the normal structure wand instead!", Color.Red);
		}

		public override void OnConfirmRectangle()
		{
			structureCache.Add(LegacySaver.SaveStructure(new Rectangle(TopLeft.X, TopLeft.Y, Width - 1, Height - 1)));
			Main.NewText("Structures to save: " + structureCache.Count);
		}
	}
}