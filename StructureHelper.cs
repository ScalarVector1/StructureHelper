
using System.Collections.Generic;
using System.Reflection;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System;

namespace StructureHelper
{
	public class StructureHelper : Mod
	{
		public StructureHelper()
		{
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
                        try
                        {
                            Type tileType = Type.GetType(d.Tile);
                            var getType = typeof(ModContent).GetMethod("TileType", BindingFlags.Static | BindingFlags.Public);
                            type = (int)getType.MakeGenericMethod(tileType).Invoke(null, null);
                        }
                        catch { type = 0; }
                    }

                    if (!int.TryParse(d.Wall, out int wallType))
                    {
                        try
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
                        tile.inActive(!d.Actuated);
                        tile.liquid = d.Liquid;
                        tile.liquidType(d.LiquidType);
                        tile.color(d.Color);
                        tile.wallColor(d.WallColor);
                        tile.wire(d.Wire[0] > 0);
                        tile.wire2(d.Wire[1] > 0);
                        tile.wire3(d.Wire[2] > 0);
                        tile.wire4(d.Wire[3] > 0);
                        tile.active(d.Active);
                    }

                }
            }
        }
    }
}