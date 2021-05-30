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
    class ChestCustomizerState : UIState
    {
        public static bool Visible;

        internal UIList ruleElements = new UIList();
        internal UIScrollbar scrollBar = new UIScrollbar();

        UIImageButton NewGuaranteed = new UIImageButton(GetTexture("StructureHelper/GUI/PlusR"));
        UIImageButton NewChance = new UIImageButton(GetTexture("StructureHelper/GUI/PlusG"));
        UIImageButton NewPool = new UIImageButton(GetTexture("StructureHelper/GUI/PlusP"));
        UIImageButton NewPoolChance = new UIImageButton(GetTexture("StructureHelper/GUI/PlusB"));

        public override void OnInitialize()
		{
            ManualGeneratorMenu.SetDims(ruleElements, -200, 0.5f, 0, 0.1f, 400, 0, 0, 0.8f);
            ManualGeneratorMenu.SetDims(scrollBar, 232, 0.5f, 0, 0.1f, 32, 0, 0, 0.8f);
            ruleElements.SetScrollbar(scrollBar);
            Append(ruleElements);
            Append(scrollBar);

            ManualGeneratorMenu.SetDims(NewGuaranteed, -240, 0.5f, 0, 0.1f, 32, 0, 32, 0);
            NewGuaranteed.OnClick += (n, m) => ruleElements.Add(new GuaranteedRuleElement());
            Append(NewGuaranteed);

            ManualGeneratorMenu.SetDims(NewChance, -240, 0.5f, 40, 0.1f, 32, 0, 32, 0);
            NewChance.OnClick += (n, m) => ruleElements.Add(new ChanceRuleElement());
            Append(NewChance);

            ManualGeneratorMenu.SetDims(NewPool, -240, 0.5f, 80, 0.1f, 32, 0, 32, 0);
            NewPool.OnClick += (n, m) => ruleElements.Add(new PoolRuleElement());
            Append(NewPool);

            ManualGeneratorMenu.SetDims(NewPoolChance, -240, 0.5f, 120, 0.1f, 32, 0, 32, 0);
            NewPoolChance.OnClick += (n, m) => ruleElements.Add(new PoolChanceRuleElement());
            Append(NewPoolChance);
        }

		public void SetData(ChestEntity entity)
        {
            entity.rules.Clear();
            for(int k = 0; k < ruleElements.Count; k++)
            {
                entity.rules.Add((ruleElements._items[k] as ChestRuleElement).rule);
            }
        }

		public override void Draw(SpriteBatch spriteBatch)
		{
            Recalculate();

            var rect = ruleElements.GetDimensions().ToRectangle();
            rect.Inflate(30, 10);
            ManualGeneratorMenu.DrawBox(spriteBatch, rect, new Color(20, 40, 60) * 0.8f);

            if (NewGuaranteed.IsMouseHovering)
                Main.hoverItemName = "Add New Guaranteed Rule";

            if (NewChance.IsMouseHovering)
                Main.hoverItemName = "Add New Chance Rule";

            if (NewPool.IsMouseHovering)
                Main.hoverItemName = "Add New Pool Rule";

            if (NewPoolChance.IsMouseHovering)
                Main.hoverItemName = "Add New Pool + Chance Rule";

            base.Draw(spriteBatch);
		}
	}
}
