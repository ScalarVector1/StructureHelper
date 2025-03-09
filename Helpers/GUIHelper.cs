using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace StructureHelper.Helpers
{
	internal static class GUIHelper
	{
		/// <summary>
		/// Draws a simple box in the style of the StructureHelper GUI.
		/// </summary>
		/// <param name="spriteBatch">the spriteBatch to draw the box with</param>
		/// <param name="target">where/how big the box should be drawn</param>
		/// <param name="color"><the color of the box/param>
		public static void DrawBox(SpriteBatch spriteBatch, Rectangle target, Color color = default)
		{
			Texture2D tex = Assets.GUI.Box.Value;

			if (color == default)
				color = new Color(49, 84, 141) * 0.9f;

			var sourceCorner = new Rectangle(0, 0, 6, 6);
			var sourceEdge = new Rectangle(6, 0, 4, 6);
			var sourceCenter = new Rectangle(6, 6, 4, 4);

			Rectangle inner = target;
			inner.Inflate(-6, -6);

			spriteBatch.Draw(tex, inner, sourceCenter, color);

			spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
		}

		/// <summary>
		/// Draws the outline of a box in the style of the DragonLens GUI.
		/// </summary>
		/// <param name="spriteBatch">the spriteBatch to draw the outline with</param>
		/// <param name="target">where/how big the outline should be drawn</param>
		/// <param name="color">the color of the outline</param>
		public static void DrawOutline(SpriteBatch spriteBatch, Rectangle target, Color color = default)
		{
			Texture2D tex = Assets.GUI.Box.Value;

			if (color == default)
				color = new Color(49, 84, 141) * 0.9f;

			var sourceCorner = new Rectangle(0, 0, 6, 6);
			var sourceEdge = new Rectangle(6, 0, 4, 6);
			var sourceCenter = new Rectangle(6, 6, 4, 4);

			spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
		}

		/// <summary>
		/// Gets the inverse of a color. Used for 'constrasting' elements of the StructureHelper GUI, such as outlines indicating something being on.
		/// </summary>
		/// <param name="color">the color to invert</param>
		/// <returns>the inverted color</returns>
		public static Color InvertColor(this Color color)
		{
			return new Color(255 - color.R, 255 - color.G, 255 - color.B, color.A);
		}

		/// <summary>
		/// Wraps a string to a given maximum width, by forcibly adding newlines. Normal newlines will be removed, put the text 'NEWBLOCK' in your string to break a paragraph if needed.
		/// </summary>
		/// <param name="input">The input string to be wrapped</param>
		/// <param name="length">The maximum width of the text</param>
		/// <param name="font">The font the text will be drawn in, to calculate its size</param>
		/// <param name="scale">The scale the text will be drawn at, to calculate its size</param>
		/// <returns>Input text with linebreaks inserted so it obeys the width constraint.</returns>
		public static string WrapString(string input, int length, DynamicSpriteFont font, float scale)
		{
			string output = "";

			// In case input is empty and causes an error, we put an empty string to the list
			var words = new List<string> { "" };

			// Word splitting, with CJK characters being treated as a single word
			string cacheString = "";
			for (int i = 0; i < input.Length; i++)
			{
				// By doing this we split words, and make the first character of words always a space
				if (cacheString != string.Empty && char.IsWhiteSpace(input[i]))
				{
					words.Add(cacheString);
					cacheString = "";
				}

				// Single CJK character just get directly added to the list
				if (LocalizationHelper.IsCjkCharacter(input[i]))
				{
					if (cacheString != string.Empty)
					{
						words.Add(cacheString);
						cacheString = "";
					}

					// If the next character is a CJK punctuation, we add both characters as a single word
					// Unless the next character is a right close CJK punctuation (e.g. left brackets), in which case we add only the current character
					if (i + 1 < input.Length && LocalizationHelper.IsCjkPunctuation(input[i + 1]) && !LocalizationHelper.IsRightCloseCjkPunctuation(input[i + 1]))
					{
						words.Add(input[i].ToString() + input[i + 1]);
						i++;
					}
					else
					{
						words.Add(input[i].ToString());
					}

					continue;
				}

				cacheString += input[i];
			}

			// Add the last word
			if (!string.IsNullOrEmpty(cacheString))
			{
				words.Add(cacheString);
			}

			string line = "";
			foreach (string str in words)
			{
				if (str == " NEWBLOCK")
				{
					output += "\n\n";
					line = "";
					continue;
				}

				if (str == " NEWLN")
				{
					output += "\n";
					line = "";
					continue;
				}

				if (font.MeasureString(line).X * scale < length)
				{
					output += str;
					line += str;
				}
				else
				{
					// We don't want the first character of a line to be a space
					output += "\n" + str.TrimStart();
					line = str;
				}
			}

			return output;
		}

		/// <summary>
		/// Uses reflection to forcibly open the TModLoader configuration UI to a given ModConfig's screen.
		/// </summary>
		/// <param name="config">The config to open up</param>
		public static void OpenConfig(ModConfig config)
		{
			IngameFancyUI.CoverNextFrame();
			Main.playerInventory = false;
			Main.editChest = false;
			Main.npcChatText = "";
			Main.inFancyUI = true;

			Type interfaceType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.Interface");
			FieldInfo modConfigList = interfaceType.GetField("modConfigList", BindingFlags.Static | BindingFlags.NonPublic);

			Type uiModConfig = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig");
			FieldInfo modConfig = interfaceType.GetField("modConfig", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo setMod = uiModConfig.GetMethod("SetMod", BindingFlags.Instance | BindingFlags.NonPublic);

			var ui = (UIState)modConfig.GetValue(null);

			setMod.Invoke(ui, new object[] { ModContent.GetInstance<StructureHelper>(), config });
			Main.InGameUI.SetState(ui);
		}

		/// <summary>
		/// Opens a URL in the web browser
		/// </summary>
		/// <param name="url">The URL to open to</param>
		public static void OpenUrl(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", url);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start("open", url);
				}
				else
				{
					throw;
				}
			}
		}
	}
}