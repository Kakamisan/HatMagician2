using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class BrandedOpening() : HatMagician2Card(13, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.Red;
    public override int BaseBrandColorCost => 13;
    public override bool HasBrandApplyTarget => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(33, ValueProp.Move), new EnergyVar(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Erosion];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonAoeApplyTargetPower<BrandRedPower>(choiceContext, 1);
        await this.CommonAoeAttack(choiceContext, play);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Energy.UpgradeValueBy(1);

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is BrandPower && power.Owner.Side != this.Owner.Creature.Side && applier == this.Owner.Creature)
        {
            var dec = this.DynamicVars.Energy.IntValue;
            this.AddBrandColorCostThisCombat = -dec;
            this.EnergyCost.AddThisCombat(-dec);
        }

        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}