using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Starlight() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    public override bool HasEndTurn => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardSelectorPrefs prefs1 = new CardSelectorPrefs(this.SelectionScreenPrompt, 0, this.DynamicVars.Cards.IntValue);
        var list1 = (await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Discard.GetPile(this.Owner).Cards, this.Owner, prefs1)).ToList();
        await CardPileCmd.Add(list1, PileType.Hand);
        
        CardSelectorPrefs prefs2 = new CardSelectorPrefs(this.SelectionScreenPrompt2, 0, this.DynamicVars.Cards.IntValue);
        var list2 = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs2, RetainFilter, this)).ToList();
        foreach (CardModel cardModel in list2) cardModel.GiveSingleTurnRetain();
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);

    private static bool RetainFilter(CardModel card) => !card.ShouldRetainThisTurn;
}