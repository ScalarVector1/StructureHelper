using System;
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
        public bool HalfSlab;
        public bool HasActuator;
        public bool Actuated;
        public byte Liquid;
        public byte LiquidType;
        public byte Color;
        public byte WallColor;
        public byte[] Wire;
        public TileSaveData(bool active, string tile, string wall, short frameX, short frameY, short wFrameX, short wFrameY, byte slope, bool halfSlab, bool hasActuator, bool actuated, byte liquid, byte liquidType, byte color, byte wallColor, byte[] wire)
        {
            Active = active;
            Tile = tile;
            Wall = wall;
            FrameX = frameX;
            FrameY = frameY;
            WFrameX = wFrameX;
            WFrameY = wFrameY;
            Slope = slope;
            HalfSlab = halfSlab;
            HasActuator = hasActuator;
            Actuated = actuated;
            Liquid = liquid;
            LiquidType = liquidType;
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
            tag.GetBool("HalfSlab"),
            tag.GetBool("HasActuator"),
            tag.GetBool("Actuated"),
            tag.GetByte("Liquid"),
            tag.GetByte("LiquidType"),
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
                ["HalfSlab"] = HalfSlab,
                ["HasActuator"] = HasActuator,
                ["Actuated"] = Actuated,
                ["Liquid"] = Liquid,
                ["LiquidType"] = LiquidType,
                ["Color"] = Color,
                ["WallColor"] = WallColor,
                ["Wire"] = Wire
            };
        }
    }
}
