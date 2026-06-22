using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace HatMagician2.HatMagician2Code.Monsters;

public class PollutedProfessorEncounter() : CustomEncounterModel(RoomType.Boss)
{
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<PollutedProfessor>().ToMutable(), "professor")
    ];

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
    [
        ModelDb.Monster<PollutedProfessor>(), ModelDb.Monster<PollutedProfessorFireBall>(), ModelDb.Monster<PollutedProfessorLightningBall>(),
        ModelDb.Monster<PollutedProfessorIceBall>(),
    ];

    public override bool IsValidForAct(ActModel act) => false;

    // public override bool HasScene => false;

    // public override string CustomRunHistoryIconPath => "";
    // public override string CustomRunHistoryIconOutlinePath => "";

    public override string BossNodePath => Path.Join(MainFile.ResPath, "anim", "map", "boss_node_polluted_professor.tres");

    public override string CustomScenePath => "encounters/polluted_professor_boss.tscn".ScenePath();

    public override IReadOnlyList<string> Slots => ["professor", "fire", "lightning", "ice"];
}