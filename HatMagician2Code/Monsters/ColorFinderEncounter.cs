using BaseLib.Abstracts;
using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Character;
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

    // 联机默认开启？
    public override bool IsValidForAct(ActModel act) =>
        act.ActNumber() == 3 && (Hat2ModConfig.ChallengeColorFinder || !RunManager.Instance.IsSingleplayerOrFakeMultiplayer);

    public override bool HasScene => false;

    // public override string CustomRunHistoryIconPath => "";
    // public override string CustomRunHistoryIconOutlinePath => "";

    public override string BossNodePath => Path.Join(MainFile.ResPath, "anim", "map", "boss_node_color_finder.tres");
}