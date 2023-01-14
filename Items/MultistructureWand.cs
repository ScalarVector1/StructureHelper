using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper.Items
{
	class MultistructureWand : ModItem
	{
		public bool secondPoint;
		public Point16 topLeft;
		public int width;
		public int height;
		internal List<TagCompound> structureCache = new();

		public Rectangle Target => new(topLeft.X, topLeft.Y, width, height);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Multistructure Wand");
			Tooltip.SetDefault("Select 2 points in the world, then right click to add a structure. Right click in your inventory when done to save.");
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override ModItem Clone(Item newEntity)
		{
			var clone = base.Clone(newEntity) as MultistructureWand;
			clone.structureCache = new List<TagCompound>();
			return clone;
		}

		public override void RightClick(Player player)
		{
			Item.stack++;

			if (structureCache.Count > 1)
				Saver.SaveMultistructureToFile(ref structureCache);
			else
				Main.NewText("Not enough structures! If you want to save a single structure, use the normal structure wand instead!", Color.Red);
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2 && !secondPoint && topLeft != default)
			{
				structureCache.Add(Saver.SaveStructure(Target));
			}
			else if (!secondPoint)
			{
				topLeft = (Main.MouseWorld / 16).ToPoint16();
				width = 0;
				height = 0;
				Main.NewText("Select Second Point");
				secondPoint = true;
			}
			else
			{
				var bottomRight = (Main.MouseWorld / 16).ToPoint16();
				width = bottomRight.X - topLeft.X - 1;
				height = bottomRight.Y - topLeft.Y - 1;
				Main.NewText("Ready to add! Right click to add this structure, Right click in inventory to save all structures");
				secondPoint = false;
			}

			return true;
		}
	}
}
