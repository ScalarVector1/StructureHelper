﻿using StructureHelper.Items;
using Terraria.DataStructures;

namespace StructureHelper
{
	public class UIRenderer : ModSystem
	{
		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
			if (Main.LocalPlayer.HeldItem.ModItem is StructureWand)
			{
				spriteBatch.End();
				spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

				Texture2D tex = ModContent.Request<Texture2D>("StructureHelper/corner").Value;
				Texture2D tex2 = ModContent.Request<Texture2D>("StructureHelper/box").Value;
				Point16 TopLeft = (Main.LocalPlayer.HeldItem.ModItem as StructureWand).topLeft;
				int Width = (Main.LocalPlayer.HeldItem.ModItem as StructureWand).width;
				int Height = (Main.LocalPlayer.HeldItem.ModItem as StructureWand).height;

				float tileScale = 16 * Main.GameViewMatrix.Zoom.Length() * 0.707106688737f;
				Vector2 pos = (Main.MouseWorld / tileScale).ToPoint16().ToVector2() * tileScale - Main.screenPosition;
				pos = Vector2.Transform(pos, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
				pos = Vector2.Transform(pos, Main.UIScaleMatrix);

				spriteBatch.Draw(tex, pos, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);

				if (Width != 0 && TopLeft != default)
				{
					spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.15f);
					spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Red, 0, tex.Frame().Size() / 2, 1, 0, 0);
				}

				if (TopLeft != default)
					spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);

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
