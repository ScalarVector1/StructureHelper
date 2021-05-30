using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StructureHelper.GUI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace StructureHelper.ChestHelper.GUI
{
    class LootElement : UIElement
    {
        Loot loot;
        UIImageButton removeButton = new UIImageButton(GetTexture("StructureHelper/GUI/Cross"));

        NumberSetter min;
        NumberSetter max;
        NumberSetter weight;

        public LootElement(Loot loot, bool hasWeight)
        {
            this.loot = loot;

            Width.Set(350, 0);
            Height.Set(36, 0);
            Left.Set(50, 0);

            removeButton.Left.Set(-36, 1);
            removeButton.Width.Set(32, 0);
            removeButton.Height.Set(32, 0);
            removeButton.OnClick += Remove;
            Append(removeButton);

            min = new NumberSetter(loot.min, "Minimum", 80);
            Append(min);

            max = new NumberSetter(loot.max, "Maximum", 120);
            Append(max);

            if (hasWeight)
            {
                weight = new NumberSetter(loot.weight, "Weight", 160);
                Append(weight);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            var target = new Rectangle((int)pos.X, (int)pos.Y, (int)GetDimensions().Width, 32);

            var color = Color.White;

            if (Parent.Parent.Parent is ChestRuleElement)
                color = (Parent.Parent.Parent as ChestRuleElement).color;

            if (removeButton.IsMouseHovering)
                Main.hoverItemName = "Remove item";

            ManualGeneratorMenu.DrawBox(spriteBatch, target, color);

            spriteBatch.Draw(Helper.GetItemTexture(loot.LootItem), new Rectangle((int)pos.X + 8, (int)pos.Y + 8, 16, 16), Color.White);

            string name = loot.LootItem.Name.Length > 25 ? loot.LootItem.Name.Substring(0, 23) + "..." : loot.LootItem.Name;
            Utils.DrawBorderString(spriteBatch, name, pos + new Vector2(28, 10), Color.White, 0.7f);

            if (min.Value > max.Value)
                min.Value = max.Value;

            if (max.Value < min.Value)
                max.Value = min.Value;

            loot.min = min.Value;
            loot.max = max.Value;

            if (weight != null)
                loot.weight = weight.Value;

            base.Draw(spriteBatch);
        }

        private void Remove(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(Parent.Parent.Parent is ChestRuleElement)) return;

            ChestRuleElement parent = Parent.Parent.Parent as ChestRuleElement;
            parent.RemoveItem(loot, this);
        }
    }
}
