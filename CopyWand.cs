using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Reflection;
using Terraria.ModLoader.IO;
using Terraria.ID;

namespace StructureHelper
{
    class StructureSaver : ModItem
    {
        public bool SecondPoint { get; set; }
        public Point16 TopLeft { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Structure Wand");
            Tooltip.SetDefault("Select 2 points in the world, then right click to save a structure");
        }
        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 20;
            item.useAnimation = 20;
        }
        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.DirtBlock, 1);
            r.AddRecipe();          
        }
        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && !SecondPoint && TopLeft != null)
            {
                SaveStructure(new Rectangle(TopLeft.X, TopLeft.Y, Width, Height));
            }

            else if (!SecondPoint)
            {
                TopLeft = (Main.MouseWorld / 16).ToPoint16();
                Width = 0;
                Height = 0;
                Main.NewText("Select Second Point");
                SecondPoint = true;
            }

            else
            {
                Point16 bottomRight = (Main.MouseWorld / 16).ToPoint16();
                Width = bottomRight.X - TopLeft.X;
                Height = bottomRight.Y - TopLeft.Y;
                Main.NewText("Ready to save! Right click to save this structure...");
                SecondPoint = false;
            }

            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = ModContent.GetTexture("StructureHelper/corner");
            Texture2D tex2 = ModContent.GetTexture("StructureHelper/box");
            if (Width != 0 && TopLeft != null)
            {
                spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.25f);
                spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Red, 0, tex.Frame().Size() / 2, 1, 0, 0);
            }
            if (TopLeft != null) spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);
        }
        private void SaveStructure(Rectangle target)
        {
            string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string thisPath = path + "/" + "SavedStructure_" + DateTime.Now.ToString("d-M-y----H-m-s-f");
            Main.NewText("Structure saved as " + thisPath, Color.Yellow);
            FileStream stream = File.Create(thisPath);
            stream.Close();

            TagCompound tag = new TagCompound();
            tag.Add("Width", Width);
            tag.Add("Height", Height);

            List<TileSaveData> data = new List<TileSaveData>();
            for (int x = target.X; x <= target.X + target.Width; x++)
            {
                for (int y = target.Y; y <= target.Y + target.Height; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    string tileName;
                    string wallName;
                    if (tile.type > Main.maxTileSets) tileName = ModContent.GetModTile(tile.type).GetType().AssemblyQualifiedName;
                    else tileName = tile.type.ToString();
                    if (tile.wall > Main.maxWallTypes) wallName = ModContent.GetModWall(tile.wall).GetType().AssemblyQualifiedName;
                    else wallName = tile.wall.ToString();

                    byte[] wireArray = new byte[]
                    {
                        (byte)tile.wire().ToInt(),
                        (byte)tile.wire2().ToInt(),
                        (byte)tile.wire3().ToInt(),
                        (byte)tile.wire4().ToInt()
                    };
                    data.Add(new TileSaveData(tile.active(), tileName, wallName, tile.frameX, tile.frameY, (short)tile.wallFrameX(), (short)tile.wallFrameY(), tile.slope(), tile.liquid, tile.color(), tile.wallColor(), wireArray));
                }
            }
            tag.Add("TileData", data);

            TagIO.ToFile(tag, thisPath);
        }
    }   
}
