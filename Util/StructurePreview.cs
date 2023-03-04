using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper.Util
{
	/// <summary>
	/// Container for a structure's preview image. 
	/// </summary>
	internal class StructurePreview : IDisposable
	{
		private readonly TagCompound tag;

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
		public int Width => preview.Width;

		/// <summary>
		/// Height of the preview texture, in pixels
		/// </summary>
		public int Height => preview.Height;

		public StructurePreview(string name, TagCompound structure)
		{
			this.name = name;
			tag = structure;

			preview = GeneratePreview();
		}

		/// <summary>
		/// Renders and saves the actual preview to a RenderTarget.
		/// </summary>
		/// <returns>The texture created</returns>
		private Texture2D GeneratePreview()
		{
			int width = tag.GetInt("Width");
			int height = tag.GetInt("Height");

			var data = (List<TileSaveData>)tag.GetList<TileSaveData>("TileData");

			RenderTargetBinding[] oldTargets = Main.graphics.GraphicsDevice.GetRenderTargets();

			RenderTarget2D newTexture = new(Main.graphics.GraphicsDevice, width * 16, height * 16);
			Main.graphics.GraphicsDevice.SetRenderTarget(newTexture);
			Main.graphics.GraphicsDevice.Clear(Color.Transparent);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int index = y + x * (height + 1);
					TileSaveData d = data[index];

					if (!int.TryParse(d.tile, out int type))
					{
						string[] parts = d.tile.Split();

						if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind<ModTile>(parts[1], out ModTile modTileType))
							type = modTileType.Type;
						else
							type = 0;
					}

					Texture2D tex = Terraria.GameContent.TextureAssets.Tile[type].Value;
					Main.spriteBatch.Draw(tex, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(d.frameX, d.frameY, 16, 16), Color.White);
				}
			}

			Main.graphics.GraphicsDevice.SetRenderTargets(oldTargets);
			return newTexture;
		}

		public void Dispose()
		{
			preview?.Dispose();
		}
	}
}

