using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Linq;
using StructureHelper.ChestHelper;

namespace StructureHelper
{
	internal static class LegacyGenerator
	{
        [Obsolete]
        public static void GenerateStructure(string path, Point16 pos, Mod mod, bool fullPath = false)
        {
            TagCompound tag;

            if (!fullPath) tag = TagIO.FromStream(mod.GetFileStream(path));
            else tag = TagIO.FromFile(path);

            if (tag == null) throw new Exception("Path to structure was unable to be found. Are you passing the correct path?");
            Generate(tag, pos);
        }

        [Obsolete]
        internal static void GenerateDebugStructure(string path, Point16 pos, Mod mod, bool fullPath = false)
        {
            TagCompound tag;

            if (!fullPath) tag = TagIO.FromStream(mod.GetFileStream(path));
            else tag = TagIO.FromFile(path);

            if (tag == null) throw new Exception("Path to structure was unable to be found. Are you passing the correct path?");
            Generate(tag, pos, true);
        }

        [Obsolete]
        public static void GenerateMultistructureRandom(string path, Point16 pos, Mod mod, bool fullPath = false, bool ignoreNull = false)
        {
            TagCompound tag;

            if (!fullPath) tag = TagIO.FromStream(mod.GetFileStream(path));
            else tag = TagIO.FromFile(path);

            if (tag == null) throw new Exception("Path to structure was unable to be found. Are you passing the correct path?");

            List<TagCompound> structures = (List<TagCompound>)tag.GetList<TagCompound>("Structures");
            int index = WorldGen.genRand.Next(structures.Count);
            TagCompound targetStructure = structures[index];
            Generate(targetStructure, pos, ignoreNull);
        }

        [Obsolete]
        public static void GenerateMultistructureSpecific(string path, Point16 pos, Mod mod, int index, bool fullPath = false, bool ignoreNull = false)
        {
            TagCompound tag;

            if (!fullPath) tag = TagIO.FromStream(mod.GetFileStream(path));
            else tag = TagIO.FromFile(path);

            if (tag == null) throw new Exception("Path to structure was unable to be found. Are you passing the correct path?");

            TagCompound targetStructure = ((List<TagCompound>)tag.GetList<TagCompound>("Structures"))[index];
            Generate(targetStructure, pos, ignoreNull);
        }

        [Obsolete]
        internal static void Generate(TagCompound tag, Point16 pos, bool ignoreNull = false)
        {
            List<LegacyTileSaveData> data = (List<LegacyTileSaveData>)tag.GetList<LegacyTileSaveData>("TileData");
            if (data == null) throw new Exception("Corrupt or Invalid structure data.");

            for (int x = 0; x <= tag.GetInt("Width"); x++)
            {
                for (int y = 0; y <= tag.GetInt("Height"); y++)
                {
                    bool isNullTile = false;
                    bool isNullWall = false;
                    int index = y + x * (tag.GetInt("Height") + 1);
                    LegacyTileSaveData d = data[index];
                    Tile tile = Framing.GetTileSafely(pos.X + x, pos.Y + y);

                    if (!int.TryParse(d.Tile, out int type))
                    {
                        string[] parts = d.Tile.Split();
                        if (parts[0] == "StructureHelper" && parts[1] == "NullBlock" && !ignoreNull) isNullTile = true;

                        else if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TileType(parts[1]) != 0)
                            type = ModLoader.GetMod(parts[0]).TileType(parts[1]);

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
                        if (parts[0] == "StructureHelper" && parts[1] == "NullWall" && !ignoreNull) isNullWall = true;

                        else if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).WallType(parts[1]) != 0)
                            wallType = ModLoader.GetMod(parts[0]).WallType(parts[1]);

                        else try
                            {
                                Type wallTypeType = Type.GetType(d.Wall); //I am so sorry for this name
                                var getWallType = typeof(ModContent).GetMethod("WallType", BindingFlags.Static | BindingFlags.Public);
                                wallType = (int)getWallType.MakeGenericMethod(wallTypeType).Invoke(null, null);
                            }
                            catch { wallType = 0; }
                    }


                    if (!d.Active) isNullTile = false;
                    if (!isNullTile || ignoreNull) //leave everything else about the tile alone if its a null block
                    {
                        tile.ClearEverything();
                        tile.type = (ushort)type;
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
                        if (!d.Active) tile.inActive(false);

                        if (d.TEType != "") //place and load a tile entity
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

                    if (!isNullWall || ignoreNull) //leave the wall alone if its a null wall
                    {
                        tile.wall = (ushort)wallType;
                        tile.wallFrameX(d.WFrameX);
                        tile.wallFrameY(d.WFrameY);
                    }

                }
            }
        }
    }
}
