using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Discharge() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApplyTarget => true;

    public override bool HasFreeBrandApplyTarget => true;

    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new RepeatVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        for (int i = 0; i < this.DynamicVars.Repeat.IntValue; i++)
        {
            Creature? enemy = this.Owner.RunState.Rng.CombatTargets.NextItem(this.CombatState.HittableEnemies);
            if (enemy != null)
            {
                await BrandPower.ApplyBrandPower(this, choiceContext, enemy, this.BaseBrandColor);
            }
        }

        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        Creature? enemy = this.Owner.RunState.Rng.CombatTargets.NextItem(this.CombatState.HittableEnemies);
        if (enemy != null)
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, enemy, this.BaseBrandColor);
        }

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Repeat.UpgradeValueBy(1);
}