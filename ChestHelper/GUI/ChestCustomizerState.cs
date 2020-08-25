using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace StructureHelper.ChestHelper.GUI
{ 
    class ChestCustomizerState : UIState
    {
        internal UIList ruleElements = new UIList();
        internal UIScrollbar scrollBar = new UIScrollbar();

        public void SetData(ChestEntity entity)
        {
            entity.rules.Clear();
            for(int k = 0; k < ruleElements.Count; k++)
            {
                entity.rules.Add((ruleElements._items[k] as ChestRuleElement).rule);
            }
        }

    }

    class ChestRuleElement : UIElement
    {
        internal ChestRule rule;
        internal UIList lootElements = new UIList();

        public override void OnInitialize()
        {
            lootElements.Left.Set(10, 0);
            lootElements.Top.Set(32, 0);
            lootElements.Width.Set(GetDimensions().Width - 60, 0);
            lootElements.Height.Set(60, 0);
            base.Append(lootElements);
        }

        public override void Click(UIMouseEvent evt)
        {
            if (Main.mouseItem.IsAir) return;

            AddItem(Main.mouseItem);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D backTex = GetTexture("StructureHelper/ChestHelper/GUI/Assets/RuleElement");
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            spriteBatch.Draw(backTex, pos, Color.White);

            base.Draw(spriteBatch);
        }

        //These handle adding/removing the elements and items from the appropriate lists, as well as re-sizing the element.
        public void AddItem(Item item)
        {
            rule.AddItem(item);

            var element = new LootElement(new Loot(item, 1));
            lootElements.Add(element);
            lootElements.Height.Set(lootElements.Height.Pixels + element.Height.Pixels, 0);
            Height.Set(Height.Pixels + element.Height.Pixels, 0);
        }

        public void RemoveItem(Loot loot, LootElement element)
        {
            rule.RemoveItem(loot);
            lootElements.Remove(element);
            lootElements.Height.Set(lootElements.Height.Pixels - element.Height.Pixels, 0);
            Height.Set(Height.Pixels - element.Height.Pixels, 0);
        }
    }

    class LootElement : UIElement
    {
        Loot loot;
        UIImageButton removeButton = new UIImageButton(GetTexture("StructureHelper/ChestHelper/GUI/Assets/Cross"));

        public LootElement(Loot loot) => this.loot = loot;

        public override void OnInitialize()
        {
            removeButton.Left.Set(-36, 1);
            removeButton.Width.Set(32, 0);
            removeButton.Height.Set(32, 0);
            removeButton.OnClick += Remove;
            base.Append(removeButton);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D backTex = GetTexture("StructureHelper/ChestHelper/GUI/Assets/LootElement");
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            spriteBatch.Draw(backTex, pos, Color.White);

            spriteBatch.Draw(Helper.GetItemTexture(loot.LootItem), new Rectangle((int)pos.X, (int)pos.Y, 32, 32), Color.White);
            Utils.DrawBorderString(spriteBatch, loot.LootItem.Name, pos + new Vector2(36, 8), Color.White);

            Utils.DrawBorderString(spriteBatch, loot.min.ToString(), pos + Vector2.UnitY * 60, Color.White);

            base.Draw(spriteBatch);
        }

        private void Remove(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(Parent is ChestRuleElement)) return;

            ChestRuleElement parent = Parent as ChestRuleElement;
            parent.RemoveItem(loot, this);
        }
    }
}
