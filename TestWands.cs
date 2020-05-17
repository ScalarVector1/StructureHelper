using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StructureHelper
{
    class TestWandCopy : CopyWand
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Structure Wand");
            Tooltip.SetDefault("Select 2 points in the world, then right click to save a structure as the test structure.\nTHIS WILL OVERRIDE THE OLD TEST STRUCTURE");
        }
        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && !SecondPoint && TopLeft != null)
            {
                SaveStructure(new Rectangle(TopLeft.X, TopLeft.Y, Width, Height), ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCache");
            }

            else if (!SecondPoint)
            {
                TopLeft = (Main.MouseWorld / 16).ToPoint16();
                Width = 0;
                Height = 0;
                Main.NewText("Select Second Point");
                SecondPoint = true;
            }

            else
            {
                Point16 bottomRight = (Main.MouseWorld / 16).ToPoint16();
                Width = bottomRight.X - TopLeft.X;
                Height = bottomRight.Y - TopLeft.Y;
                Main.NewText("Ready to save! Right click to save this structure to the test cache...");
                SecondPoint = false;
            }

            return true;
        }
    }
    class TestWandPaste : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Structure Spawning Wand");
            Tooltip.SetDefault("Use to spawn the currently stored test structure.");
        }
        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 20;
            item.useAnimation = 20;
            item.rare = 1;
        }
        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.DirtBlock, 1);
            r.SetResult(this);
            r.AddRecipe();
        }
        public override bool UseItem(Player player)
        {
            StructureHelper.GenerateStructure(ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCache", (Main.MouseWorld / 16).ToPoint16(), mod, true);
            return true;
        }
    }
}
