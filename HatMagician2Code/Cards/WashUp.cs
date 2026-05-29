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
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(5)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep, CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override bool IsPlayableSub => PileType.Draw.GetPile(this.Owner).Cards.Count > 0 || PileType.Discard.GetPile(this.Owner).Cards.Count > 0;

    protected override bool ShouldGlowRedInternal => !this.IsPlayable;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.ShuffleIfNecessary(choiceContext, this.Owner);
        IReadOnlyList<CardModel> cards = PileType.Draw.GetPile(this.Owner).Cards.ToList().Take(this.DynamicVars.Cards.IntValue).ToList();
        CardModel? card2 = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, this.Owner, new CardSelectorPrefs(this.SelectionScreenPrompt, 1))).FirstOrDefault();
        if (card2 == null)
            return;
        await CardPileCmd.Add(card2, PileType.Hand);
        foreach (var otherCard in cards)
        {
            if (otherCard != card2 && otherCard.Keywords.Contains(HatMagician2Keywords.Sleep))
            {
                await CardCmd.Discard(choiceContext, otherCard);
            }
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(2);
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}