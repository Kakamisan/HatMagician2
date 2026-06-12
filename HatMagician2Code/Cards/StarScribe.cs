using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class StarScribe() : HatMagician2Card(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    private int AccDamage => CardStateSingleton.GetColorCost(this.Owner) * this.DynamicHat2Var.IntValue;

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new DamageVar(1, ValueProp.Move), new("StarScribeBase", 1), new("StarScribeExtra", 1), new Hat2Var(1),
        new CustomCalculatedDamageVar("StarScribe", ValueProp.Move).WithMultiplier((card, _) => ((StarScribe)card).AccDamage)
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonSingleAttack(choiceContext, play, (CustomCalculatedDamageVar)this.GetDynamicVar("StarScribe"));
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}