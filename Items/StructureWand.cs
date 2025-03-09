using StructureHelper.GUI;
using StructureHelper.Models;
using StructureHelper.Saving;
using Terraria.DataStructures;
using Terraria.ID;

namespace StructureHelper.Items
{
	class StructureWand : ModItem
	{
		public bool secondPoint;

		public Point16 point1;
		public Point16 point2;

		public bool movePoint1;
		public bool movePoint2;

		public Point16 TopLeft => new(point1.X < point2.X ? point1.X : point2.X, point1.Y < point2.Y ? point1.Y : point2.Y);
		public Point16 BottomRight => new(point1.X > point2.X ? point1.X : point2.X, point1.Y > point2.Y ? point1.Y : point2.Y);
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
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override void HoldItem(Player player)
		{
			if (movePoint1)
				point1 = (Main.MouseWorld / 16).ToPoint16();

			if (movePoint2)
				point2 = (Main.MouseWorld / 16).ToPoint16();

			if (!Main.mouseLeft)
			{
				movePoint1 = false;
				movePoint2 = false;
			}
		}

		/// <summary>
		/// What happens when you right click after a finazed rectangle is present
		/// </summary>
		public virtual void OnConfirmRectangle()
		{
			NameConfirmPopup.OpenConfirmation((name) =>
			{
				Saver.SaveToFile(StructureData.FromWorld(TopLeft.X, TopLeft.Y, Width, Height), null, name);
			});
			
			
			//NameConfirmPopup.OpenConfirmation((name) => LegacySaver.SaveToFile(new Rectangle(TopLeft.X, TopLeft.Y, Width - 1, Height - 1), name: name));
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2 && Ready)
			{
				OnConfirmRectangle();
				return true;
			}

			if (Ready)
			{
				if (Vector2.Distance(Main.MouseWorld, point1.ToVector2() * 16) <= 32)
				{
					movePoint1 = true;
					return true;
				}

				if (Vector2.Distance(Main.MouseWorld, point2.ToVector2() * 16) <= 32)
				{
					movePoint2 = true;
					return true;
				}
			}

			if (!secondPoint)
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