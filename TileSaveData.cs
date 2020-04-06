using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
    public struct TileSaveData : TagSerializable
    {
        public bool Active;
        public string Tile;
        public string Wall;
        public short FrameX;
        public short FrameY;
        public short WFrameX;
        public short WFrameY;
        public byte Slope;
        public byte Liquid;
        public byte Color;
        public byte WallColor;
        public byte[] Wire;
        public TileSaveData(bool active, string tile, string wall, short frameX, short frameY, short wFrameX, short wFrameY, byte slope, byte liquid, byte color, byte wallColor, byte[] wire)
        {
            Active = active;
            Tile = tile;
            Wall = wall;
            FrameX = frameX;
            FrameY = frameY;
            WFrameX = wFrameX;
            WFrameY = wFrameY;
            Slope = slope;
            Liquid = liquid;
            Color = color;
            WallColor = wallColor;
            Wire = wire;
        }
        public static Func<TagCompound, TileSaveData> DESERIALIZER = s => DeserializeData(s);
        public static TileSaveData DeserializeData(TagCompound tag)
        {
            return new TileSaveData(
            tag.GetBool("Active"),
            tag.GetString("Tile"),
            tag.GetString("Wall"),
            tag.GetShort("FrameX"),
            tag.GetShort("FrameY"),
            tag.GetShort("WFrameX"),
            tag.GetShort("WFrameY"),
            tag.GetByte("Slope"),
            tag.GetByte("Liquid"),
            tag.GetByte("Color"),
            tag.GetByte("WallColor"),
            tag.GetByteArray("Wire")
            );
        }

        public TagCompound SerializeData()
        {
            return new TagCompound()
            {
                ["Active"] = Active,
                ["Tile"] = Tile,
                ["Wall"] = Wall,
                ["FrameX"] = FrameX,
                ["FrameY"] = FrameY,
                ["WFrameX"] = WFrameX,
                ["WFrameY"] = WFrameY,
                ["Slope"] = Slope,
                ["Liquid"] = Liquid,
                ["Color"] = Color,
                ["WallColor"] = WallColor,
                ["Wire"] = Wire
            };
        }
    }
}
