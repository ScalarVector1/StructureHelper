using StructureHelper.GUI;
using Terraria.DataStructures;
using Terraria.ID;

namespace StructureHelper.Items
{
	class TestWand : ModItem
	{
		public static bool ignoreNulls = false;
		public static bool UIVisible;

		public override void Load()
		{
			Terraria.On_Main.DrawPlayers_AfterProjectiles += DrawPreview;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Structure Placer Wand");
			Tooltip.SetDefault("left click to place the selected structure, right click to open the structure selector");
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				ManualGeneratorMenu.LoadStructures();
				UIVisible = !UIVisible;
				return true;
			}

			if (ManualGeneratorMenu.selected != null)
			{
				var pos = new Point16(Player.tileTargetX, Player.tileTargetY);

				if (ManualGeneratorMenu.multiMode)
					Generator.GenerateMultistructureSpecific(ManualGeneratorMenu.selected.path, pos, StructureHelper.Instance, ManualGeneratorMenu.multiIndex, true, ManualGeneratorMenu.ignoreNulls, ManualGeneratorMenu.flags);
				else
					Generator.GenerateStructure(ManualGeneratorMenu.selected.path, pos, StructureHelper.Instance, true, ManualGeneratorMenu.ignoreNulls, ManualGeneratorMenu.flags);
			}
			else
			{
				Main.NewText("No structure selected! Right click and select a structure from the menu to generate it.", Color.Red);
			}

			return true;
		}

		private void DrawPreview(Terraria.On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
		{
			orig(self);

			if (ManualGeneratorMenu.selected != null && ManualGeneratorMenu.preview != null && Main.LocalPlayer.HeldItem.type == Item.type)
			{
				Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

				var pos = new Point16(Player.tileTargetX, Player.tileTargetY);
				Vector2 pos2 = pos.ToVector2() * 16 - Main.screenPosition;

				Helpers.GUIHelper.DrawBox(Main.spriteBatch, new Rectangle((int)pos2.X - 4, (int)pos2.Y - 4, ManualGeneratorMenu.preview.Width + 8, ManualGeneratorMenu.preview.Height + 8), Color.Red * 0.5f);

				if (ManualGeneratorMenu.preview?.preview != null)
					Main.spriteBatch.Draw(ManualGeneratorMenu.preview?.preview, pos2, Color.White * 0.5f);

				Main.spriteBatch.End();
			}
		}
	}
}