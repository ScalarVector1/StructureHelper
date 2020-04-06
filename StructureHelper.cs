
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
        public static void GenerateStructure(string path, Point16 pos, Mod mod)
        {
            TagCompound tag = TagIO.FromStream(mod.GetFileStream(path));
            List<TileSaveData> data = (List<TileSaveData>)tag.GetList<TileSaveData>("TileData");
            for (int x = 0; x <= tag.GetInt("Width"); x++)
            {
                for (int y = 0; y <= tag.GetInt("Height"); y++)
                {
                    int index = y + x * (tag.GetInt("Height") + 1);
                    TileSaveData d = data[index];
                    Tile tile = Framing.GetTileSafely(pos.X + x, pos.Y + y);

                    tile.ClearEverything();
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
                            type = (int)getWallType.MakeGenericMethod(wallTypeType).Invoke(null, null);
                        }
                        catch { type = 0; }
                    }
                    tile.type = (ushort)type;
                    tile.wall = (ushort)wallType;
                    tile.frameX = d.FrameX;
                    tile.frameY = d.FrameY;
                    tile.wallFrameX(d.WFrameX);
                    tile.wallFrameY(d.WFrameY);
                    tile.slope(d.Slope);
                    tile.liquid = d.Liquid;
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