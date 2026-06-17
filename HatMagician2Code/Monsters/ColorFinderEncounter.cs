using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Acts;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Monsters;

public class ColorFinderEncounter() : CustomEncounterModel(RoomType.Boss)
{
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<ColorFinderPainting>().ToMutable(), null),
        (ModelDb.Monster<ColorFinder>().ToMutable(), null),
    ];

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<ColorFinderPainting>(), ModelDb.Monster<ColorFinder>(),];

    public override bool IsValidForAct(ActModel act) => act is ActPaintingWorld;

    // 联机默认开启？
    public static bool IsValidForAct(RunState runState) =>
        runState.Players.Any(p => PileType.Deck.GetPile(p).Cards.Any(c => c is BlankPainting))
        && Hat2ModConfigUtil.ShouldActiveColorFinder(runState);

    public override bool HasScene => false;

    // public override string CustomRunHistoryIconPath => "";
    // public override string CustomRunHistoryIconOutlinePath => "";

    public override string BossNodePath => Path.Join(MainFile.ResPath, "anim", "map", "boss_node_color_finder.tres");
}