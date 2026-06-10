using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace HatMagician2.HatMagician2Code.Monsters;

public class ColorFinderEncounter() : CustomEncounterModel(RoomType.Boss)
{
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<ColorFinderPainting>().ToMutable(), null),
        (ModelDb.Monster<ColorFinder>().ToMutable(), null),
    ];

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<ColorFinderPainting>(), ModelDb.Monster<ColorFinder>(),];
    public override bool IsValidForAct(ActModel act) => act.ActNumber() == 999;
    public override bool HasScene => false;
}