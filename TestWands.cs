using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StructureHelper
{
    class TestWandCopy : CopyWand
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Structure Wand");
            Tooltip.SetDefault("Select 2 points in the world, then right click to save a structure as the test structure.\nthis will override the old test structure.");
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && !SecondPoint && TopLeft != null)
            {
                SaveStructure(new Rectangle(TopLeft.X, TopLeft.Y, Width, Height), ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCache");
                StructureHelper.Instance.PreviewWidth = Width;
                StructureHelper.Instance.PreviewHeight = Height;
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
                Width = bottomRight.X - TopLeft.X - 1;
                Height = bottomRight.Y - TopLeft.Y - 1;
                Main.NewText("Ready to save! Right click to save this structure to the test cache...");
                SecondPoint = false;
            }

            return true;
        }
    }

    class TestWandPaste : ModItem
    {
        bool ignoreNull = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Structure Spawning Wand");
            Tooltip.SetDefault("Use to spawn the currently stored test structure.");
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 20;
            item.useAnimation = 20;
            item.rare = 1;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                ignoreNull = !ignoreNull;
                if (ignoreNull) Main.NewText("Now ignoring null tiles (Edit Mode)");
                else Main.NewText("Now respecting null tiles (Test Mode)");
                return true;
            }

            if (ignoreNull) StructureHelper.GenerateDebugStructure(ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCache", (Main.MouseWorld / 16).ToPoint16(), mod, true);
            else StructureHelper.GenerateStructure(ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCache", (Main.MouseWorld / 16).ToPoint16(), mod, true);

            return true;
        }
    }

    class MultiWandCopy : MultiWand
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Multitructure Wand");
            Tooltip.SetDefault("Use to spawn the currently stored test multistructure.");
        }

        public override void RightClick(Player player)
        {
            item.stack++;
            if (StructureCache.Count > 1) SaveMeForGood(ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCacheMulti");
            else Main.NewText("Not enough structures! If you want to test a single structure, use the normal structure test wand instead!", Color.Red);
        }
    }

    class MultiWandPaste : ModItem
    {
        bool ignoreNull = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Multitructure Spawning Wand");
            Tooltip.SetDefault("Use to spawn the currently stored test multistructure.");
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 20;
            item.useAnimation = 20;
            item.rare = 1;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                ignoreNull = !ignoreNull;
                if (ignoreNull) Main.NewText("Now ignoring null tiles and placing all variants. This will take up alot of horizontal space ;) (Edit Mode)");
                else Main.NewText("Now respecting null tiles and placing random variant (Test Mode)");
                return true;
            }

            if (ignoreNull)
            {
                Point16 pos = (Main.MouseWorld / 16).ToPoint16();

                TagCompound tag = TagIO.FromFile(ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCacheMulti");

                if (tag == null) throw new Exception("Path to structure was unable to be found. Are you passing the correct path?");

                List<TagCompound> structures = (List<TagCompound>)tag.GetList<TagCompound>("Structures");

                for(int k = 0; k < structures.Count; k++)
                {
                    StructureHelper.Generate(structures[k], pos);
                    pos = new Point16(pos.X + structures[k].GetInt("Width") + 2, pos.Y);
                }
                
            }

            else StructureHelper.GenerateMultistructureRandom(ModLoader.ModPath.Replace("Mods", "SavedStructures") + "/TestWandCacheMulti", (Main.MouseWorld / 16).ToPoint16(), mod, true);

            return true;
        }
    }
}
