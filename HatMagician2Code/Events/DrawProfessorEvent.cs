using HatMagician2.HatMagician2Code.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Events;

public sealed class DrawProfessorEvent : Hat2Event
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(333), new("Gold2", 132)];

    public override bool IsAllowed(IRunState runState) => false;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        this.Owner!.Deck.Cards.Any(c => c is BlankPainting) ? Option(this.PickRelic) : LockedOption("LOCK"),
        this.Owner!.Deck.Cards.Any(c => c is BlankPainting) ? Option(this.PickGold) : LockedOption("LOCK"),
        Option(this.KeepCard)
    ];

    private async Task PickRelic()
    {
        await this.RemoveAllBlankPaintings();
        await RewardsCmd.OfferCustom(this.Owner!, [new RelicReward(this.Owner!), new RelicReward(this.Owner!)]);
        SetEventFinished(PageDescription("PICK_RELIC"));
    }

    private async Task PickGold()
    {
        await this.RemoveAllBlankPaintings();
        await PlayerCmd.GainGold(this.DynamicVars.Gold.BaseValue, this.Owner!);
        SetEventFinished(PageDescription("PICK_GOLD"));
    }

    private async Task KeepCard()
    {
        await RewardsCmd.OfferCustom(this.Owner!, [new GoldReward(this.DynamicVars["Gold2"].IntValue, this.Owner!), new PotionReward(this.Owner!)]);
        SetEventFinished(PageDescription("KEEP_CARD"));
    }

    private async Task RemoveAllBlankPaintings()
    {
        foreach (var cardModel in this.Owner!.Deck.Cards.Where(c => c is BlankPainting).ToList())
        {
            PlayerCmd.CompleteQuest(cardModel);
            await CardPileCmd.RemoveFromDeck(cardModel);
        }
    }
}