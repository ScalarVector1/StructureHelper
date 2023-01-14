using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
	public struct TileSaveData : TagSerializable
	{
		public string tile;
		public string wall;
		public short frameX;
		public short frameY;
		public int wallWireData;
		public short packedLiquidData;

		public string TEType;
		public TagCompound TEData;

		public static Func<TagCompound, TileSaveData> DESERIALIZER = s => DeserializeData(s);

		public bool Active => TileDataPacking.GetBit(wallWireData, 0);

		public TileSaveData(string tile, string wall, short frameX, short frameY, int wallWireData, short packedLiquidData, string teType = "", TagCompound teData = null)
		{

			this.tile = tile;
			this.wall = wall;
			this.frameX = frameX;
			this.frameY = frameY;
			this.wallWireData = wallWireData;
			this.packedLiquidData = packedLiquidData;
			TEType = teType;
			TEData = teData;
		}

		public static TileSaveData DeserializeData(TagCompound tag)
		{
			var output = new TileSaveData(
			tag.GetString("Tile"),
			tag.GetString("Wall"),
			tag.GetShort("FrameX"),
			tag.GetShort("FrameY"),

			tag.GetInt("WallWireData"),
			tag.GetShort("PackedLiquidData")
			);

			if (tag.ContainsKey("TEType"))
			{
				output.TEType = tag.GetString("TEType");
				output.TEData = tag.Get<TagCompound>("TEData");
			}

			return output;
		}

		public TagCompound SerializeData()
		{
			var tag = new TagCompound()
			{
				["Tile"] = tile,
				["Wall"] = wall,
				["FrameX"] = frameX,
				["FrameY"] = frameY,

				["WallWireData"] = wallWireData,
				["PackedLiquidData"] = packedLiquidData
			};

			if (TEType != "")
			{
				tag.Add("TEType", TEType);
				tag.Add("TEData", TEData);
			}

			return tag;
		}
	}
}
