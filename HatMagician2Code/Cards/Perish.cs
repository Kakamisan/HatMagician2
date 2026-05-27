using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Perish() : HatMagician2Card(0, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    public override bool HasBrandColorCostX => true;
    protected override bool HasEnergyCostX => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(9, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).WithHitCount(this.ResolveEnergyXValue() + this.ResolveBrandColorCostXValue()).FromCard(this)
            .TargetingRandomOpponents(this.CombatState).WithHitFx("vfx/vfx_starry_impact").Execute(choiceContext);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(4);
}