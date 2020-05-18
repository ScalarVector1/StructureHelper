
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
    public class StructureHelper : Mod
    {
        public StructureHelper() { }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem.modItem is CopyWand)
            {
                spriteBatch.End();
                spriteBatch.Begin();

                Texture2D tex = ModContent.GetTexture("StructureHelper/corner");
                Texture2D tex2 = ModContent.GetTexture("StructureHelper/box");
                Point16 TopLeft = (Main.LocalPlayer.HeldItem.modItem as CopyWand).TopLeft;
                int Width = (Main.LocalPlayer.HeldItem.modItem as CopyWand).Width;
                int Height = (Main.LocalPlayer.HeldItem.modItem as CopyWand).Height;

                if (Width != 0 && TopLeft != null)
                {
                    spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.25f);
                    spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Red, 0, tex.Frame().Size() / 2, 1, 0, 0);
                }
                if (TopLeft != null) spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);
            }
        }

        /// <summary>
        /// This method generates a structure from a file within your mod.
        /// </summary>
        /// <param name="path">The path to your structure file within your mod - this should not include your mod's folder, only the path beyond it.</param>
        /// <param name="pos">The position in the world in which you want your structure to generate, in tile coordinates.</param>
        /// <param name="mod">The instance of your mod to grab the file from. This should almost always be YourModClass.Instance.</param>
        /// ///<param name="fullPath">Indicates if you want to use a fully qualified path to get the structure file instead of one from your mod - generally should only used for debug purposes.</param>
        public static void GenerateStructure(string path, Point16 pos, Mod mod, bool fullPath = false)
        {
            TagCompound tag;
            List<TileSaveData> data;
            if (!fullPath)
            {
                tag = TagIO.FromStream(mod.GetFileStream(path));
                data = (List<TileSaveData>)tag.GetList<TileSaveData>("TileData");
            }
            else
            {
                tag = TagIO.FromFile(path);
                data = (List<TileSaveData>)tag.GetList<TileSaveData>("TileData");
            }
            if (tag == null || data == null) throw new Exception("Path to structure was unable to be found. Are you passing the correct path?");

            for (int x = 0; x <= tag.GetInt("Width"); x++)
            {
                for (int y = 0; y <= tag.GetInt("Height"); y++)
                {
                    int index = y + x * (tag.GetInt("Height") + 1);
                    TileSaveData d = data[index];
                    Tile tile = Framing.GetTileSafely(pos.X + x, pos.Y + y);

                    if (!int.TryParse(d.Tile, out int type))
                    {
                        string[] parts = d.Tile.Split();
                        if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TileType(parts[1]) != 0)
                        {
                            type = ModLoader.GetMod(parts[0]).TileType(parts[1]);
                        }

                        else try
                        {
                            Type tileType = Type.GetType(d.Tile);
                            var getType = typeof(ModContent).GetMethod("TileType", BindingFlags.Static | BindingFlags.Public);
                            type = (int)getType.MakeGenericMethod(tileType).Invoke(null, null);
                        }
                        catch { type = 0; }
                    }

                    if (!int.TryParse(d.Wall, out int wallType))
                    {
                        string[] parts = d.Wall.Split();
                        if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).WallType(parts[1]) != 0)
                        {
                            wallType = ModLoader.GetMod(parts[0]).WallType(parts[1]);
                        }

                        else try
                        {
                            Type wallTypeType = Type.GetType(d.Wall); //I am so sorry for this name
                            var getWallType = typeof(ModContent).GetMethod("WallType", BindingFlags.Static | BindingFlags.Public);
                            wallType = (int)getWallType.MakeGenericMethod(wallTypeType).Invoke(null, null);
                        }
                        catch { wallType = 0; }
                    }

                    if (wallType != ModContent.WallType<NullWall>())
                    {
                        tile.wall = (ushort)wallType; //leave the wall alone if its a null wall
                        tile.wallFrameX(d.WFrameX);
                        tile.wallFrameY(d.WFrameY);
                    }

                    if (type != ModContent.TileType<NullBlock>())
                    {
                        tile.type = (ushort)type; //leave everything else about the tile alone if its a null block
                        tile.frameX = d.FrameX;
                        tile.frameY = d.FrameY;
                        tile.slope(d.Slope);
                        tile.halfBrick(d.HalfSlab);
                        tile.actuator(d.HasActuator);
                        tile.inActive(d.Actuated);
                        tile.liquid = d.Liquid;
                        tile.liquidType(d.LiquidType);
                        tile.color(d.Color);
                        tile.wallColor(d.WallColor);
                        tile.wire(d.Wire[0] > 0);
                        tile.wire2(d.Wire[1] > 0);
                        tile.wire3(d.Wire[2] > 0);
                        tile.wire4(d.Wire[3] > 0);
                        tile.active(d.Active);

                        if(d.TEType != "") //place and load a tile entity
                        {
                            int typ;

                            if (!int.TryParse(d.TEType, out typ))
                            {
                                string[] parts = d.TEType.Split();
                                typ = ModLoader.GetMod(parts[0]).TileEntityType(parts[1]);
                            }

                            if (d.TEType != "")
                            {
                                TileEntity.PlaceEntityNet(pos.X + x, pos.Y + y, typ);
                                if (d.TEData != null && typ > 2) (TileEntity.ByPosition[new Point16(pos.X + x, pos.Y + y)] as ModTileEntity).Load(d.TEData);
                            }
                        }
                    }

                }
            }
        }
    }
}