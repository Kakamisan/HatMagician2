using BaseLib.Abstracts;
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

    public override bool IsValidForAct(ActModel act) => false;

    public override bool HasScene => false;

    // public override string CustomRunHistoryIconPath => "";
    // public override string CustomRunHistoryIconOutlinePath => "";

    public override string BossNodePath => Path.Join(MainFile.ResPath, "anim", "map", "boss_node_color_finder.tres");
}