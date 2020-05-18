using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
    class CopyWand : ModItem
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
            item.rare = 1;
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
        public void SaveStructure(Rectangle target, string targetPath = null)
        {
            string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string thisPath = targetPath ?? Path.Combine(path, "SavedStructure_" + DateTime.Now.ToString("d-M-y----H-m-s-f"));
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
                    string teName;
                    if (tile.type > Main.maxTileSets) tileName = ModContent.GetModTile(tile.type).mod.Name + " " + ModContent.GetModTile(tile.type).Name;
                    else tileName = tile.type.ToString();
                    if (tile.wall > Main.maxWallTypes) wallName = ModContent.GetModWall(tile.wall).mod.Name + " " + ModContent.GetModWall(tile.wall).Name;
                    else wallName = tile.wall.ToString();

                    TileEntity teTarget = null; //grabbing TE data
                    TagCompound entityTag = null;

                    if (TileEntity.ByPosition.ContainsKey(new Point16(x, y))) teTarget = TileEntity.ByPosition[new Point16(x, y)];

                    if (teTarget != null)
                    {
                        if (teTarget.type < 2)
                        {
                            teName = teTarget.type.ToString();
                        }
                        else
                        {
                            ModTileEntity entityTarget = ModTileEntity.GetTileEntity(teTarget.type);
                            if (entityTarget != null)
                            {
                                teName = entityTarget.mod.Name + " " + entityTarget.Name;
                                entityTag = (teTarget as ModTileEntity).Save();
                            }
                            else teName = "";
                        }
                    }
                    else teName = "";

                    byte[] wireArray = new byte[]
                    {
                        (byte)tile.wire().ToInt(),
                        (byte)tile.wire2().ToInt(),
                        (byte)tile.wire3().ToInt(),
                        (byte)tile.wire4().ToInt()
                    };
                    data.Add(new TileSaveData(tile.active(), tileName, wallName, tile.frameX, tile.frameY, (short)tile.wallFrameX(), (short)tile.wallFrameY(),
                        tile.slope(), tile.halfBrick(), tile.actuator(), !tile.nactive(), tile.liquid, tile.liquidType(), tile.color(), tile.wallColor(), wireArray, 
                        teName, entityTag));
                }
            }
            tag.Add("TileData", data);

            TagIO.ToFile(tag, thisPath);
        }
    }
}
