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
public class Stumble() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(3)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, 2);
        var cards1 = (await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner, prefs, null, this)).ToList();
        await CardCmd.Discard(choiceContext, cards1);
        
        await CardPileCmd.ShuffleIfNecessary(choiceContext, this.Owner);
        CardSelectorPrefs prefs2 = new CardSelectorPrefs(this.SelectionScreenPrompt2, 1);
        IReadOnlyList<CardModel> cards = PileType.Draw.GetPile(this.Owner).Cards.ToList().Take(this.DynamicVars.Cards.IntValue).ToList();
        CardModel? card2 = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, this.Owner, prefs2)).FirstOrDefault();
        if (card2 == null)
            return;
        //await CardPileCmd.Add(card2, PileType.Hand);
        await CardCmd.AutoPlay(choiceContext, card2, null);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(2);
}