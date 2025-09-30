using StructureHelper.Models;
using System;
using System.Collections.Generic;

namespace StructureHelper.Util
{
	public class StructurePreview
	{
		public StructureData data;

		/// <summary>
		/// The name of the structure this is previewing
		/// </summary>
		public readonly string name;

		/// <summary>
		/// The actual texture of the preview
		/// </summary>
		public Texture2D preview;

		/// <summary>
		/// Width of the preview texture, in pixels
		/// </summary>
		public int Width => preview?.Width ?? 1;

		/// <summary>
		/// Height of the preview texture, in pixels
		/// </summary>
		public int Height => preview?.Height ?? 1;

		public StructurePreview(string name, StructureData data)
		{
			this.name = name;
			this.data = data;

			PreviewRenderQueue.queue.Add(this);
		}

		/// <summary>
		/// Renders and saves the actual preview to a RenderTarget.
		/// </summary>
		/// <returns>The texture created</returns>
		internal unsafe Texture2D GeneratePreview()
		{
			int width = Math.Min(data.width, 8096 / 16);
			int height = Math.Min(data.height, 8096 / 16);
			
			RenderTargetBinding[] oldTargets = Main.graphics.GraphicsDevice.GetRenderTargets();

			RenderTarget2D newTexture = new(Main.graphics.GraphicsDevice, width * 16, height * 16, false, default, default, default, RenderTargetUsage.PreserveContents);

			Main.graphics.GraphicsDevice.SetRenderTarget(newTexture);

			Main.graphics.GraphicsDevice.Clear(Color.Transparent);

			Main.spriteBatch.Begin();

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					TileWallWireStateData wireStateData = *(TileWallWireStateData*)data.dataEntries["Terraria/TileWallWireStateData"].GetSingleEntry(x, y);
					ushort tileType = (*(TileTypeData*)data.dataEntries["Terraria/TileTypeData"].GetSingleEntry(x, y)).Type;
					ushort wallType = (*(WallTypeData*)data.dataEntries["Terraria/WallTypeData"].GetSingleEntry(x, y)).Type;
					int frameX = wireStateData.TileFrameX;
					int frameY = wireStateData.TileFrameY;
					bool hasTile = wireStateData.HasTile;

					if (wallType != 0 && wallType != StructureHelper.NULL_IDENTIFIER && wallType < Terraria.GameContent.TextureAssets.Wall.Length)
					{
						Texture2D tex = Terraria.GameContent.TextureAssets.Wall[wallType].Value;
						Main.spriteBatch.Draw(tex, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(8, 8, 16, 16), Color.White);
					}

					if (hasTile && tileType != StructureHelper.NULL_IDENTIFIER && tileType < Terraria.GameContent.TextureAssets.Tile.Length)
					{
						Texture2D tex = Terraria.GameContent.TextureAssets.Tile[tileType].Value;
						Main.spriteBatch.Draw(tex, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(frameX, frameY, 16, 16), Color.White);
					}
				}
			}

			Main.spriteBatch.End();

			Main.graphics.GraphicsDevice.SetRenderTargets(null);
			Main.graphics.GraphicsDevice.SetRenderTargets(oldTargets);
			return newTexture;
		}

		public void Dispose()
		{
			preview?.Dispose();
		}
	}

	public class PreviewRenderQueue : ILoadable
	{
		/// <summary>
		/// A queue of previews to render textures for when the next opportunity arises
		/// </summary>
		public static List<StructurePreview> queue = [];

		public void Load(Mod mod)
		{
			On_Main.CheckMonoliths += DrawQueuedPreview;
		}

		public void Unload()
		{

		}

		/// <summary>
		/// When the opportunity in the rendering cycle arises, render out all of the queued previews
		/// </summary>
		/// <param name="orig"></param>
		private void DrawQueuedPreview(On_Main.orig_CheckMonoliths orig)
		{
			foreach (StructurePreview queued in queue)
			{
				queued.preview = queued.GeneratePreview();
			}

			queue.Clear();

			orig();
		}
	}
}
