using BaseLib.Utils.NodeFactories;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;
using HatMagician2.HatMagician2Code.Intents;
using HatMagician2.HatMagician2Code.MonsterPowers;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code.Monsters;

public class PollutedProfessorIceBall : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 129, 99);

    public override NCreatureVisuals CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("monsters/ice_ball.tscn".ScenePath());

    private static int BaseDebuff1 => 3;
    private static int BaseDebuff2 => 3;

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1M, this.Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var debuffMove = new MoveState("DEBUFF", this.DebuffMove, new DebuffIntent());
        var debuffMove2 = new MoveState("DEBUFF2", this.DebuffMove2, new DebuffIntent());
        var debuffMove3 = new MoveState("DEBUFF3", this.DebuffMove3, new BrandBlueIntent());

        debuffMove.FollowUpState = debuffMove2;
        debuffMove2.FollowUpState = debuffMove3;
        debuffMove3.FollowUpState = debuffMove;

        List<MonsterState> states = [debuffMove, debuffMove2, debuffMove3];

        return new MonsterMoveStateMachine(states, debuffMove);
    }

    private async Task DebuffMove(IReadOnlyList<Creature> targets)
    {
        foreach (var target in targets)
        {
            await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), target, BaseDebuff1, this.Creature, null);
        }
    }

    private async Task DebuffMove2(IReadOnlyList<Creature> targets)
    {
        foreach (var target in targets)
        {
            await PowerCmd.Apply<PollutedIceBallDexPower>(new ThrowingPlayerChoiceContext(), target, -BaseDebuff2, this.Creature, null);
        }
    }

    private async Task DebuffMove3(IReadOnlyList<Creature> targets)
    {
        foreach (var target in targets)
        {
            await BrandPower.ApplyBrandPower(null, this.Creature, new ThrowingPlayerChoiceContext(), target, BrandColor.Blue);
        }
    }
}