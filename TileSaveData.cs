using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
	/// <summary>
	/// A struct representing tile data to be saved/loaded from structure files.
	/// </summary>
	public struct TileSaveData : TagSerializable
	{
		/// <summary>
		/// The tile to be placed, either a number if a vanilla tile (ID), or a fully qualified internal name for modded tiles.
		/// </summary>
		public string tile;
		/// <summary>
		/// The wall to be placed, either a number if a vanilla wall (ID), or a fully qualified internal name for modded walls.
		/// </summary>
		public string wall;
		/// <summary>
		/// The X frame of a tile
		/// </summary>
		public short frameX;
		/// <summary>
		/// the Y frame of a tile
		/// </summary>
		public short frameY;
		/// <summary>
		/// One part of the packed vanilla data about a tile
		/// </summary>
		public int wallWireData;
		/// <summary>
		/// The other part of the packed vanilla data about a tile
		/// </summary>
		public short packedLiquidData;

		/// <summary>
		/// The fully qualiified name of a modded tile entity, if one should exist here
		/// </summary>
		public string TEType;
		/// <summary>
		/// The data associated with a tile entity associated here
		/// </summary>
		public TagCompound TEData;

		public static Func<TagCompound, TileSaveData> DESERIALIZER = s => DeserializeData(s);

		/// <summary>
		/// If the tile here is air or not. (Note that TileID 0 is dirt, this is all that differentiates air and dirt.)
		/// </summary>
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

		/// <summary>
		/// Deserialize a TagCompound into an instance of this struct
		/// </summary>
		/// <param name="tag">The tag to interpret</param>
		/// <returns>The unpacked TileSaveData</returns>
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

		/// <summary>
		/// Serialize this struct into a TagCompound for saving
		/// </summary>
		/// <returns>The packed TagCompound</returns>
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
