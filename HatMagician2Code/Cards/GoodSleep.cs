using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class GoodSleep() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    public override int BaseBrandColorCost => 1;
    public override bool HasEndTurn => true;

    // public override bool HasBrandApply => false;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(2), new EnergyVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplySelfPower<DrawCardsNextTurnPower>(choiceContext, play, this.DynamicVars.Cards.IntValue);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplySelfPower<EnergyNextTurnPower>(choiceContext, play, this.DynamicVars.Energy.IntValue);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1);
        this.DynamicVars.Energy.UpgradeValueBy(1);
    }
}