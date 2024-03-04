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

		public Point16 point1;
		public Point16 point2;

		public Point16 TopLeft => new Point16(point1.X < point2.X ? point1.X : point2.X, point1.Y < point2.Y ? point1.Y : point2.Y);
		public Point16 BottomRight => new Point16(point1.X > point2.X ? point1.X : point2.X, point1.Y > point2.Y ? point1.Y : point2.Y);
		public int Width => BottomRight.X - TopLeft.X;
		public int Height => BottomRight.Y - TopLeft.Y;

		public bool Ready => !secondPoint && point1 != default;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Structure Wand");
			Tooltip.SetDefault("Select 2 points in the world, then right click to save a structure");
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
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
			if (player.altFunctionUse == 2 && !secondPoint && TopLeft != default)
			{
				Saver.SaveToFile(new Rectangle(TopLeft.X, TopLeft.Y, Width, Height));
			}
			else if (!secondPoint)
			{
				point1 = (Main.MouseWorld / 16).ToPoint16();
				point2 = default;

				Main.NewText("Select Second Point");
				secondPoint = true;
			}
			else
			{
				point2 = (Main.MouseWorld / 16).ToPoint16();

				Main.NewText("Ready to save! Right click to save this structure...");
				secondPoint = false;
			}

			return true;
		}
	}
}
