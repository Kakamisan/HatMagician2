using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Evasion() : HatMagician2Card(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new BlockVar(10, ValueProp.Move), new EnergyVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Dream];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonBlock(play);
        await HatMagician2Mgr.AddEnergy(this.Owner, this.DynamicVars.Energy.IntValue, BrandColor.Purple);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(3);
        // this.DynamicVars.Energy.UpgradeValueBy(1);
    }

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card, bool isAutoPlay, ResourceInfo resources, PileType pileType,
        CardPilePosition position)
    {
        if (card == this)
            return pileType != PileType.Discard ? (pileType, position) : (PileType.Draw, CardPilePosition.Bottom);
        return base.ModifyCardPlayResultPileTypeAndPosition(card, isAutoPlay, resources, pileType, position);
    }
}