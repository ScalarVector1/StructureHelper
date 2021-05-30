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
    class ChestRuleElement : UIElement
    {
        internal ChestRule rule;
        internal Color color = Color.White;

        internal UIList lootElements = new UIList();
        UIImageButton removeButton = new UIImageButton(GetTexture("StructureHelper/GUI/Cross"));

        public ChestRuleElement(ChestRule rule)
        {
            this.rule = rule;

            Width.Set(400, 0);
            Height.Set(36, 0);

            lootElements.Left.Set(0, 0);
            lootElements.Top.Set(40, 0);
            lootElements.Width.Set(400, 0);
            lootElements.Height.Set(32, 0);
            Append(lootElements);

            removeButton.Left.Set(-36, 1);
            removeButton.Width.Set(32, 0);
            removeButton.Height.Set(32, 0);
            removeButton.OnClick += Remove;
            Append(removeButton);
        }

        public override void Click(UIMouseEvent evt)
        {
            if (Main.mouseItem.IsAir) return;

            AddItem(Main.mouseItem);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = GetDimensions().ToRectangle().TopLeft();
            var target = new Rectangle((int)pos.X, (int)pos.Y, (int)GetDimensions().Width, 32);
            ManualGeneratorMenu.DrawBox(spriteBatch, target, color);

            if (target.Contains(Main.MouseScreen.ToPoint()))
                Main.hoverItemName = rule.Tooltip + "\nLeft click this while holding an item to add it";

            if(removeButton.IsMouseHovering)
                Main.hoverItemName = "Remove rule";

            Utils.DrawBorderString(spriteBatch, rule.Name, pos + Vector2.One * 4, Color.White, 0.8f);

            base.Draw(spriteBatch);
        }

        //These handle adding/removing the elements and items from the appropriate lists, as well as re-sizing the element.
        public void AddItem(Item item)
        {
            var loot = rule.AddItem(item);

            var element = new LootElement(loot, rule.UsesWeight);
            lootElements.Add(element);
            lootElements.Height.Set(lootElements.Height.Pixels + element.Height.Pixels + 4, 0);
            Height.Set(Height.Pixels + element.Height.Pixels + 4, 0);
        }

        public void RemoveItem(Loot loot, LootElement element)
        {
            rule.RemoveItem(loot);
            lootElements.Remove(element);
            lootElements.Height.Set(lootElements.Height.Pixels - element.Height.Pixels - 4, 0);
            Height.Set(Height.Pixels - element.Height.Pixels - 4, 0);
        }

        private void Remove(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(Parent.Parent.Parent is ChestCustomizerState)) return;

            ChestCustomizerState parent = Parent.Parent.Parent as ChestCustomizerState;
            parent.ruleElements.Remove(this);
        }
    }
}
