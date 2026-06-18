using HatMagician2.HatMagician2Code.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Events;

public sealed class HomelessPersonEvent : Hat2Event
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(53)];

    public override bool IsAllowed(IRunState runState) => false;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        this.Owner!.Deck.Cards.Any(c => c is BlankPainting) ? Option(this.DuplicateCard) : LockedOption("LOCK"),
        this.Owner!.Deck.Cards.Any(c => c is BlankPainting) ? Option(this.RemoveCards) : LockedOption("LOCK"),
        Option(this.AddCard, tips: [HoverTipFactory.FromCard<ColorCover>()])
    ];

    private async Task DuplicateCard()
    {
        await this.RemoveAllBlankPaintings();
        CardSelectorPrefs prefs = new CardSelectorPrefs(new LocString("card_selection", "HATMAGICIAN2-TO_DUPLICATE"), 1);
        CardModel? mutableCard = (await CardSelectCmd.FromDeckGeneric(this.Owner!, prefs, this.Filter)).FirstOrDefault();
        if (mutableCard != null)
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(this.Owner!.RunState.CloneCard(mutableCard), PileType.Deck));
        SetEventFinished(PageDescription("DUPLICATE_CARD"));
    }

    private async Task RemoveCards()
    {
        await this.RemoveAllBlankPaintings();
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 2);
        await CardPileCmd.RemoveFromDeck((await CardSelectCmd.FromDeckForRemoval(this.Owner!, prefs)).ToList());
        await PlayerCmd.GainGold(this.DynamicVars.Gold.BaseValue, this.Owner!);
        SetEventFinished(PageDescription("REMOVE_CARDS"));
    }

    private async Task AddCard()
    {
        await this.RemoveAllBlankPaintings();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(this.Owner!.RunState.CreateCard<ColorCover>(this.Owner), PileType.Deck));
        SetEventFinished(PageDescription("ADD_CARD"));
    }

    private async Task RemoveAllBlankPaintings()
    {
        foreach (var cardModel in this.Owner!.Deck.Cards.Where(c => c is BlankPainting).ToList())
        {
            PlayerCmd.CompleteQuest(cardModel);
            await CardPileCmd.RemoveFromDeck(cardModel);
        }
    }

    private bool Filter(CardModel c) => c.Type != CardType.Quest;
}