using StructureHelper.Items;
using System;
using Terraria.DataStructures;

namespace StructureHelper
{
	public class UIRenderer : ModSystem
	{
		public override void Load()
		{
			On_Main.DrawInterface += DrawSelection;
		}

		private void DrawSelection(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;

			if (Main.LocalPlayer.HeldItem.ModItem is StructureWand)
			{
				var wand = (Main.LocalPlayer.HeldItem.ModItem as StructureWand);

				spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default, default, Main.GameViewMatrix.ZoomMatrix);

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

					topLeft = new Point16(point1.X < point2.X ? point1.X : point2.X, point1.Y < point2.Y ? point1.Y : point2.Y);
					bottomRight = new Point16(point1.X > point2.X ? point1.X : point2.X, point1.Y > point2.Y ? point1.Y : point2.Y);
					int Width = bottomRight.X - topLeft.X - 1;
					int Height = bottomRight.Y - topLeft.Y - 1;

					var target = new Rectangle((int)(topLeft.X * 16 - Main.screenPosition.X), (int)(topLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16);
					Helpers.GUIHelper.DrawOutline(spriteBatch, target, Color.Gold);
					spriteBatch.Draw(tex2, target, tex2.Frame(), Color.White * 0.15f);

					spriteBatch.Draw(tex, wand.point1.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);
					spriteBatch.Draw(tex, point2.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);
				}
				else if (wand.Ready)
				{
					int Width = bottomRight.X - topLeft.X - 1;
					int Height = bottomRight.Y - topLeft.Y - 1;

					var target = new Rectangle((int)(topLeft.X * 16 - Main.screenPosition.X), (int)(topLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16);
					Helpers.GUIHelper.DrawOutline(spriteBatch, target, Color.Lerp(Color.Gold, Color.White, 0.5f + 0.5f * (float)System.Math.Sin(Main.GameUpdateCount * 0.2f)));
					spriteBatch.Draw(tex2, target, tex2.Frame(), Color.White * 0.15f);

					float scale1 = Vector2.Distance(Main.MouseWorld, wand.point1.ToVector2() * 16) < 32 ? 1.5f : 1f;
					spriteBatch.Draw(tex, wand.point1.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan * scale1, 0, tex.Frame().Size() / 2, scale1, 0, 0);

					float scale2 = Vector2.Distance(Main.MouseWorld, wand.point2.ToVector2() * 16) < 32 ? 1.5f : 1f;
					spriteBatch.Draw(tex, wand.point2.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Red * scale2, 0, tex.Frame().Size() / 2, scale2, 0, 0);
				}

				spriteBatch.End();
			}

			orig(self, gameTime);
		}
	}
}
