using StructureHelper.Models.Legacy;
using System;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace StructureHelper.Util
{
	/// <summary>
	/// Container for a structure's preview image. 
	/// </summary>
	public class StructurePreview : IDisposable
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
		public int Width => preview?.Width ?? 1;

		/// <summary>
		/// Height of the preview texture, in pixels
		/// </summary>
		public int Height => preview?.Height ?? 1;

		public StructurePreview(string name, TagCompound structure)
		{
			this.name = name;
			tag = structure;

			PreviewRenderQueue.queue.Add(this);
		}

		/// <summary>
		/// Renders and saves the actual preview to a RenderTarget.
		/// </summary>
		/// <returns>The texture created</returns>
		internal Texture2D GeneratePreview()
		{
			int width = tag.GetInt("Width");
			int height = tag.GetInt("Height");

			var data = (List<LegacyTileSaveData>)tag.GetList<LegacyTileSaveData>("TileData");

			RenderTargetBinding[] oldTargets = Main.graphics.GraphicsDevice.GetRenderTargets();

			RenderTarget2D newTexture = new(Main.graphics.GraphicsDevice, (width + 1) * 16, (height + 1) * 16, false, default, default, default, RenderTargetUsage.PreserveContents);

			Main.graphics.GraphicsDevice.SetRenderTarget(newTexture);

			Main.graphics.GraphicsDevice.Clear(Color.Transparent);

			Main.spriteBatch.Begin();

			for (int x = 0; x <= width; x++)
			{
				for (int y = 0; y <= height; y++)
				{
					int index = y + x * (height + 1);
					LegacyTileSaveData d = data[index];

					if (!int.TryParse(d.tile, out int type))
					{
						string[] parts = d.tile.Split();

						if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind<ModTile>(parts[1], out ModTile modTileType))
							type = modTileType.Type;
						else
							type = 0;
					}

					if (!int.TryParse(d.wall, out int wallType))
					{
						string[] parts = d.wall.Split();

						if (parts.Length > 1 && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind<ModWall>(parts[1], out ModWall modWallType))
							wallType = modWallType.Type;
						else
							wallType = 0;
					}

					if (wallType != 0 && d.wall != "StructureHelper NullWall")
					{
						Texture2D tex = Terraria.GameContent.TextureAssets.Wall[wallType].Value;
						Main.spriteBatch.Draw(tex, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(8, 8, 16, 16), Color.White);
					}

					if (d.Active && d.tile != "StructureHelper NullBlock")
					{
						Texture2D tex = Terraria.GameContent.TextureAssets.Tile[type].Value;
						Main.spriteBatch.Draw(tex, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(d.frameX, d.frameY, 16, 16), Color.White);
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