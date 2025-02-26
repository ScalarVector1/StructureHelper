﻿using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace StructureHelper.ChestHelper
{
	class ChestEntity : ModTileEntity
	{
		public List<ChestRule> rules = [];

		public void SaveChestRulesFile()
		{
			string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string thisPath = Path.Combine(path, "SavedChest_" + DateTime.Now.ToString("d-M-y----H-m-s-f"));
			Main.NewText("Chest data saved as " + thisPath, Color.Yellow);
			FileStream stream = File.Create(thisPath);
			stream.Close();

			TagCompound tag = SaveChestRules();
			TagIO.ToFile(tag, thisPath);
		}

		public TagCompound SaveChestRules()
		{
			var tag = new TagCompound
			{
				{ "Count", rules.Count }
			};

			for (int k = 0; k < rules.Count; k++)
			{
				tag.Add("Rule" + k, rules[k].Serizlize());
			}

			return tag;
		}

		public static List<ChestRule> LoadChestRules(TagCompound tag)
		{
			var rules = new List<ChestRule>();
			int count = tag.GetInt("Count");

			for (int k = 0; k < count; k++)
			{
				rules.Add(ChestRule.Deserialize(tag.GetCompound("Rule" + k)));
			}

			return rules;
		}

		public static void SetChest(Chest chest, List<ChestRule> rules)
		{
			int index = 0;
			rules.ForEach(n => n.PlaceItems(chest, ref index));
		}

		public override void Update()
		{
			Dust.NewDustPerfect(Position.ToVector2() * 16 + Vector2.UnitY * 8 + Vector2.One.RotatedByRandom(6.28f) * 6, 111, Vector2.Zero, 0, default, 0.5f);
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("Count", rules.Count);

			for (int k = 0; k < rules.Count; k++)
			{
				tag.Add("Rule" + k, rules[k].Serizlize());
			}
		}

		public override void LoadData(TagCompound tag)
		{
			rules = LoadChestRules(tag);
		}

		public override bool IsTileValidForEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			return tile.TileType == TileID.Containers || TileID.Sets.BasicChest[tile.TileType];
		}
	}
}