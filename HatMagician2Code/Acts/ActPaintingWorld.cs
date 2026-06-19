using BaseLib.Abstracts;
using Godot;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace HatMagician2.HatMagician2Code.Acts;

public class ActPaintingWorld() : CustomActModel(3, false)
{
    public override bool IsUnlocked(UnlockState unlockState) => true;
    public override int Index => 2;
    public override bool IsDefault => true;

    public override IEnumerable<EncounterModel> GenerateAllEncounters() => [..ModelDb.Act<Glory>().AllEncounters];

    public override IEnumerable<EventModel> AllEvents => ModelDb.Act<Glory>().AllEvents;
    protected override string CustomMapTopBgPath => ModelDb.Act<Glory>().MapTopBgPath;
    protected override string CustomMapMidBgPath => ModelDb.Act<Glory>().MapMidBgPath;
    protected override string CustomMapBotBgPath => ModelDb.Act<Glory>().MapBotBgPath;
    protected override string CustomRestSiteBackgroundPath => ModelDb.Act<Glory>().RestSiteBackgroundPath;

    protected override int BaseNumberOfRooms => 14;

    public override Color MapTraveledColor => new("27221C");

    public override Color MapUntraveledColor => new("6E7750");

    public override Color MapBgColor => new("819a98");

    // patch判断是否进入画界 卡组中有空白画作/经历过学院教授事件
    public static bool IsValidForEnterWorld(RunState runState) =>
        runState.Players.Any(p => PileType.Deck.GetPile(p).Cards.Any(c => c is BlankPainting));
    // || runState.VisitedEventIds.Contains(ModelDb.Event<DrawProfessorEvent>().Id);
}