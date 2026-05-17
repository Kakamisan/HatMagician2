using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class WashUp() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(4)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep, CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override bool IsPlayable => PileType.Draw.GetPile(this.Owner).Cards.Count > 0 || PileType.Discard.GetPile(this.Owner).Cards.Count > 0;

    protected override bool ShouldGlowRedInternal => !this.IsPlayable;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var card1 = this;
        await CardPileCmd.ShuffleIfNecessary(choiceContext, this.Owner);
        IReadOnlyList<CardModel> cards = PileType.Draw.GetPile(card1.Owner).Cards.ToList().Take(card1.DynamicVars.Cards.IntValue).ToList();
        CardModel? card2 = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, card1.Owner, new CardSelectorPrefs(card1.SelectionScreenPrompt, 1))).FirstOrDefault();
        if (card2 == null)
            return;
        await CardPileCmd.Add(card2, PileType.Hand);
        foreach (var otherCard in cards)
        {
            if (otherCard != card2)
            {
                await CardCmd.Discard(choiceContext, otherCard);
            }
        }

        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(2);
}