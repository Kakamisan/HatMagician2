using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Doodle() : HatMagician2Card(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApplyTarget => true;

    public override bool HasFreeBrandApplyTarget => true;

    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(7, ValueProp.Move)];
    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        BrandColor[] list = [BrandColor.Red, BrandColor.Blue, BrandColor.Yellow, BrandColor.White, BrandColor.Orange, BrandColor.Purple];
        var color = this.Owner.RunState.Rng.CombatTargets.NextItem(list);
        await BrandPower.ApplyBrandPower(this, choiceContext, play, color);
        await this.CommonSingleAttack(choiceContext, play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3);
}