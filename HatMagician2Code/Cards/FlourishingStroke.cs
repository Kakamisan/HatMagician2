using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class FlourishingStroke() : HatMagician2Card(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasFreeBrandApplyTarget => true;
    public override bool IsAoeAttack => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(6, ValueProp.Move), new Hat2Var(2), new RepeatVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        BrandColor[] list = [BrandColor.Red, BrandColor.Blue, BrandColor.Yellow, BrandColor.White, BrandColor.Orange, BrandColor.Purple];
        for (int i = 0; i < this.DynamicHat2Var.IntValue; i++)
        {
            foreach (var e in this.CombatState.HittableEnemies)
            {
                BrandColor color;
                if (e.Powers.FirstOrDefault(p => p is BrandPower) is BrandPower power)
                {
                    color = power.BaseBrandColor;
                }
                else
                {
                    color = this.Owner.RunState.Rng.CombatTargets.NextItem(list);
                }

                await BrandPower.ApplyBrandPower(this, choiceContext, e, color);
            }
        }

        await this.CommonAoeAttack(choiceContext, play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(5);
        // this.DynamicHat2Var.UpgradeValueBy(1);
        // this.DynamicVars.Repeat.UpgradeValueBy(1);
    }
}