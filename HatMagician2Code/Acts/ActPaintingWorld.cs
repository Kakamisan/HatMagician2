using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;

namespace HatMagician2.HatMagician2Code.Acts;

public class ActPaintingWorld() : CustomActModel(3, false)
{
    public override IEnumerable<EncounterModel> GenerateAllEncounters() => [..ModelDb.Act<Glory>().AllEncounters];

    public override IEnumerable<EventModel> AllEvents => ModelDb.Act<Glory>().AllEvents;
    protected override string CustomMapTopBgPath => ModelDb.Act<Glory>().MapTopBgPath;
    protected override string CustomMapMidBgPath => ModelDb.Act<Glory>().MapMidBgPath;
    protected override string CustomMapBotBgPath => ModelDb.Act<Glory>().MapBotBgPath;
    protected override string CustomRestSiteBackgroundPath => ModelDb.Act<Glory>().RestSiteBackgroundPath;

    protected override int BaseNumberOfRooms => 13;

    public override Color MapTraveledColor => new ("27221C");

    public override Color MapUntraveledColor => new ("6E7750");

    public override Color MapBgColor => new ("819a98");
}