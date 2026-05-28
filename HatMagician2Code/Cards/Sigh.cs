using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Sigh() : HatMagician2Card(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromPower<GloomyPower>()];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new Hat2Var(6), new EnergyVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (HatMagician2Mgr.Instance == null)
            return;
        await this.CommonApplyTargetPower<GloomyPower>(choiceContext, play, this.DynamicHat2Var.IntValue);
        await HatMagician2Mgr.AddEnergy(this.Owner, this.DynamicVars.Energy.IntValue, BrandColor.Purple);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicHat2Var.UpgradeValueBy(3);
        this.DynamicVars.Energy.UpgradeValueBy(1);
    }
}