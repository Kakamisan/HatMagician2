using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Sampling() : HatMagician2Card(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(HatMagician2Keywords.Color), HoverTipFactory.FromKeyword(HatMagician2Keywords.BaseColor)];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new Hat2Var(1)];
    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    private int AddValue => this.IsUpgraded ? 1 : 0;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.Owner.Creature.Player == null)
            return;
        if (HatMagician2Mgr.Instance == null)
            return;
        List<BrandColor> list = [BrandColor.Red, BrandColor.Yellow, BrandColor.Blue];
        foreach (var color in list)
        {
            await HatMagician2Mgr.AddEnergy(this.Owner.Creature.Player, this.DynamicHat2Var.IntValue + this.AddValue, color);
        }

        List<BrandColor> list2 = [BrandColor.Orange, BrandColor.Purple, BrandColor.White];
        foreach (var color in list2)
        {
            await HatMagician2Mgr.AddEnergy(this.Owner.Creature.Player, this.DynamicHat2Var.IntValue, color);
        }

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => base.OnUpgrade();
}