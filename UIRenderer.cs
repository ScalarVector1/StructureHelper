using StructureHelper.Items;
using Terraria.DataStructures;

namespace StructureHelper
{
	public class UIRenderer : ModSystem
	{
		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
			if (Main.LocalPlayer.HeldItem.ModItem is StructureWand)
			{
				var wand = (Main.LocalPlayer.HeldItem.ModItem as StructureWand);

				spriteBatch.End();
				spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

				Texture2D tex = ModContent.Request<Texture2D>("StructureHelper/corner").Value;
				Texture2D tex2 = ModContent.Request<Texture2D>("StructureHelper/box").Value;
				
				Point16 topLeft = wand.TopLeft;
				Point16 bottomRight = wand.BottomRight;

				float tileScale = 16 * Main.GameViewMatrix.Zoom.Length() * 0.707106688737f;
				Vector2 pos = (Main.MouseWorld / tileScale).ToPoint16().ToVector2() * tileScale - Main.screenPosition;
				pos = Vector2.Transform(pos, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
				pos = Vector2.Transform(pos, Main.UIScaleMatrix);

				spriteBatch.Draw(tex, pos, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);

				if (wand.secondPoint)
				{
					var point1 = wand.point1;
					var point2 = (Main.MouseWorld / 16).ToPoint16();

					spriteBatch.Draw(tex, wand.point1.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);
					spriteBatch.Draw(tex, point2.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Red, 0, tex.Frame().Size() / 2, 1, 0, 0);

					topLeft = new Point16(point1.X < point2.X ? point1.X : point2.X, point1.Y < point2.Y ? point1.Y : point2.Y);
					bottomRight = new Point16(point1.X > point2.X ? point1.X : point2.X, point1.Y > point2.Y ? point1.Y : point2.Y);
					int Width = bottomRight.X - topLeft.X - 1;
					int Height = bottomRight.Y - topLeft.Y - 1;

					var target = new Rectangle((int)(topLeft.X * 16 - Main.screenPosition.X), (int)(topLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16);
					Helpers.GUIHelper.DrawOutline(spriteBatch, target, Color.Gold);
					spriteBatch.Draw(tex2, target, tex2.Frame(), Color.White * 0.15f);
				}
				else if (wand.Ready)
				{
					spriteBatch.Draw(tex, wand.point1.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);
					spriteBatch.Draw(tex, wand.point2.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Red, 0, tex.Frame().Size() / 2, 1, 0, 0);

					int Width = bottomRight.X - topLeft.X - 1;
					int Height = bottomRight.Y - topLeft.Y - 1;

					var target = new Rectangle((int)(topLeft.X * 16 - Main.screenPosition.X), (int)(topLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16);
					Helpers.GUIHelper.DrawOutline(spriteBatch, target, Color.Lerp(Color.Gold, Color.White, 0.5f + 0.5f * (float)System.Math.Sin(Main.GameUpdateCount * 0.2f)));
					spriteBatch.Draw(tex2, target, tex2.Frame(), Color.White * 0.15f);
				}

				spriteBatch.End();
				spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
			}

			if (Main.LocalPlayer.HeldItem.ModItem is MultistructureWand)
			{
				spriteBatch.End();
				spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

				Texture2D tex = ModContent.Request<Texture2D>("StructureHelper/corner").Value;
				Texture2D tex2 = ModContent.Request<Texture2D>("StructureHelper/box").Value;
				Point16 TopLeft = (Main.LocalPlayer.HeldItem.ModItem as MultistructureWand).topLeft;
				int Width = (Main.LocalPlayer.HeldItem.ModItem as MultistructureWand).width;
				int Height = (Main.LocalPlayer.HeldItem.ModItem as MultistructureWand).height;
				int count = (Main.LocalPlayer.HeldItem.ModItem as MultistructureWand).structureCache.Count;

				float tileScale = 16 * Main.GameViewMatrix.Zoom.Length() * 0.707106688737f;
				Vector2 pos = (Main.MouseWorld / tileScale).ToPoint16().ToVector2() * tileScale - Main.screenPosition;
				pos = Vector2.Transform(pos, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
				pos = Vector2.Transform(pos, Main.UIScaleMatrix);

				spriteBatch.Draw(tex, pos, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);

				if (Width != 0 && TopLeft != default)
				{
					spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.15f);
					spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Yellow, 0, tex.Frame().Size() / 2, 1, 0, 0);
				}

				if (TopLeft != default)
					spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.LimeGreen, 0, tex.Frame().Size() / 2, 1, 0, 0);

				spriteBatch.End();
				spriteBatch.Begin();
				Utils.DrawBorderString(spriteBatch, "Structures to save: " + count, Main.MouseScreen + new Vector2(0, 30), Color.White);

				spriteBatch.End();
				spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
			}
		}
	}
}
