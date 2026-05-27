using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Agitation() : HatMagician2Card(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    public override int BaseBrandColorCost => 3;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Chain)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new Hat2Var(2), new("AgitationExtra", 2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplySelfPower<AgitationPower>(choiceContext, play, this.DynamicHat2Var.IntValue + this.GetDynamicVar("AgitationExtra").IntValue);
        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplySelfPower<AgitationPower>(choiceContext, play, this.DynamicHat2Var.IntValue);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicHat2Var.UpgradeValueBy(1);
        this.DynamicBrandCost.UpgradeValueBy(-1);
    }
}