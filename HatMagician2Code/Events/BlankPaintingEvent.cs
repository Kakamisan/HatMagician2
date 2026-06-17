using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Events;

public class BlankPaintingEvent : Hat2Event
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(53)];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(this.PickPainting, tips: [HoverTipFactory.FromCard<BlankPainting>()]),
        Option(this.IgnorePainting)
    ];

    public override bool IsAllowed(IRunState runState) => runState.CurrentActIndex == 0 && Hat2ModConfigUtil.ShouldActiveColorFinder(runState);

    private async Task PickPainting()
    {
        await PlayerCmd.GainGold(this.DynamicVars.Gold.BaseValue, this.Owner!);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(this.Owner!.RunState.CreateCard<BlankPainting>(this.Owner), PileType.Deck), 2f);
        SetEventFinished(PageDescription("PICK_PAINTING"));
    }

    private async Task IgnorePainting()
    {
        await RelicCmd.Obtain(RelicFactory.PullNextRelicFromFront(this.Owner!).ToMutable(), this.Owner!);
        SetEventFinished(PageDescription("IGNORE_PAINTING"));
    }
}