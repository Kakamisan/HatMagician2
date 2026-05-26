using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Fluorescence() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new Hat2Var(1), new EnergyVar(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust, HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PlayerCmd.GainEnergy(this.DynamicVars.Energy.IntValue, this.Owner);
        await HatMagician2Mgr.AddEnergy(this.Owner, this.DynamicHat2Var.IntValue, BrandColor.White);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicHat2Var.UpgradeValueBy(1);

    public override Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == this.Owner.Creature.Side && this.Pile?.Type == PileType.Hand)
        {
            this.DynamicHat2Var.UpgradeValueBy(1);
        }

        return base.AfterSideTurnEndLate(choiceContext, side, participants);
    }
}