using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StructureHelper.Items
{
	class StructureWand : ModItem
	{
		public bool secondPoint;
		public Point16 topLeft;
		public int width;
		public int height;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Structure Wand");
			Tooltip.SetDefault("Select 2 points in the world, then right click to save a structure");
		}

		public override void SetDefaults()
		{
			Item.useStyle = 1;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2 && !secondPoint && topLeft != default)
			{
				Saver.SaveToFile(new Rectangle(topLeft.X, topLeft.Y, width, height));
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
				Main.NewText("Ready to save! Right click to save this structure...");
				secondPoint = false;
			}

			return true;
		}
	}
}
