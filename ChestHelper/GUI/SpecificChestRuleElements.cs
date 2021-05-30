using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureHelper.ChestHelper.GUI
{
	class GuaranteedRuleElement : ChestRuleElement
	{
		public GuaranteedRuleElement() : base(new ChestRuleGuaranteed())
		{
			color = Color.Red;
		}
	}

	class ChanceRuleElement : ChestRuleElement
	{
		NumberSetter chanceSetter = new NumberSetter(100, "Chance", 80, "%");

		public ChanceRuleElement() : base(new ChestRuleChance())
		{
			color = Color.Green;
			Append(chanceSetter);
		}

		public override void Update(GameTime gameTime)
		{
			if (chanceSetter.Value > 100)
				chanceSetter.Value = 100;

			(rule as ChestRuleChance).chance = chanceSetter.Value / 100f;
			base.Update(gameTime);
		}
	}

	class PoolRuleElement : ChestRuleElement
	{
		NumberSetter countSetter = new NumberSetter(1, "Amount to Pick", 80);

		public PoolRuleElement() : base(new ChestRulePool())
		{
			color = Color.Purple;
			Append(countSetter);
		}

		public override void Update(GameTime gameTime)
		{
			if (countSetter.Value > rule.pool.Count)
				countSetter.Value = rule.pool.Count;

			(rule as ChestRulePool).itemsToGenerate = countSetter.Value;
			base.Update(gameTime);
		}
	}

	class PoolChanceRuleElement : ChestRuleElement
	{
		NumberSetter chanceSetter = new NumberSetter(100, "Chance", 80, "%");
		NumberSetter countSetter = new NumberSetter(1, "Amount to Pick", 120);

		public PoolChanceRuleElement() : base(new ChestRulePoolChance())
		{
			color = Color.Blue;
			Append(chanceSetter);
			Append(countSetter);
		}

		public override void Update(GameTime gameTime)
		{
			if (countSetter.Value > rule.pool.Count)
				countSetter.Value = rule.pool.Count;

			if (chanceSetter.Value > 100)
				chanceSetter.Value = 100;

			(rule as ChestRulePoolChance).itemsToGenerate = countSetter.Value;
			(rule as ChestRulePoolChance).chance = chanceSetter.Value / 100f;
			base.Update(gameTime);
		}
	}
}
