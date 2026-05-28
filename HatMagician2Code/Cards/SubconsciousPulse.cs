using BaseLib.Cards.Variables;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class SubconsciousPulse() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Chain)];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new DamageVar(3, ValueProp.Unpowered), new RepeatVar(2),
        new CustomCalculatedVar("DamageShow").WithMultiplier((card, _) => ((SubconsciousPulse)card).GetDamageShow()),
        new("DamageShowBase", 3), new("DamageShowExtra", 1)
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Dream];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        await BrandPower.ChainDamageCmdFromCard(this.CombatState, this.DynamicVars.Damage.IntValue, this.Owner.Creature, this, true, this.DynamicVars.Repeat.IntValue);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Repeat.UpgradeValueBy(1);

    private decimal GetDamageShow()
    {
        if (this.CombatState == null) return 0;
        var modifyChainDamage = HatMagician2Mgr.ModifyChainDamage(null, 0, ValueProp.Unpowered, this.Owner.Creature, this, this.CombatState);
        return modifyChainDamage;
    }
}