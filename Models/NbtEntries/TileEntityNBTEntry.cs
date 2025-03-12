using StructureHelper.ChestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace StructureHelper.Models.NbtEntries
{
	internal class TileEntityNBTEntry : StructureNBTEntry
	{
		string tileEntityType;
		TagCompound tileEntityData;

		public static Func<TagCompound, TileEntityNBTEntry> DESERIALIZER = Deserialize;

		public TileEntityNBTEntry(int x, int y, string tileEntityType, TagCompound tileEntityData) : base(x, y) 
		{ 
			this.tileEntityType = tileEntityType;
			this.tileEntityData = tileEntityData;
		}

		public static TileEntityNBTEntry Deserialize(TagCompound tag)
		{
			TileEntityNBTEntry entry = new(
				tag.GetInt("x"), 
				tag.GetInt("y"),
				tag.GetString("tileEntityType"),
				tag.Get<TagCompound>("tileEntityData"));

			return entry;
		}

		//TODO: Do we want to move these to their own StructureNBTEntry?
		internal void GenerateChest(Point16 pos, TagCompound rules)
		{
			int i = Chest.CreateChest(pos.X, pos.Y);

			if (i == -1)
				return;

			Chest chest = Main.chest[i];
			ChestEntity.SetChest(chest, ChestEntity.LoadChestRules(rules));
		}

		public override void OnGenerate(Point16 generatingAt, bool ignoreNull, GenFlags flags)
		{
			if (tileEntityType == "StructureHelper/ChestEntity" && !ignoreNull)
			{
				GenerateChest(generatingAt, tileEntityData);
			}
			else if ((flags & GenFlags.IgnoreTileEnttiyData) == 0)
			{
				if (!int.TryParse(tileEntityType, out int typ))
				{
					string[] parts = tileEntityType.Split("/", 2);

					if (ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind<ModTileEntity>(parts[1], out ModTileEntity te))
						typ = te.Type;
				}

				if (typ != 0)
				{
					TileEntity.PlaceEntityNet(generatingAt.X, generatingAt.Y, typ);

					if (tileEntityData != null && typ != 2 && TileEntity.ByPosition.ContainsKey(generatingAt)) // We specifically exclude logic sensors (type 2) because these store their absolute pos
						TileEntity.ByPosition[generatingAt].LoadData(tileEntityData);
				}
			}
		}

		public override void Serialize(TagCompound tag)
		{
			tag["tileEntityType"] = tileEntityType;
			tag["tileEntityData"] = tileEntityData;
		}
	}
}
