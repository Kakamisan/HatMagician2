using BaseLib.Utils.NodeFactories;
using HatMagician2.HatMagician2Code.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code.Monsters;

public class PollutedProfessorFireBall : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 99, 86);

    public override NCreatureVisuals CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("monsters/fire_ball.tscn".ScenePath());

    private static int BaseAttack => 15;
    private static int BaseBuff => 3;
    private static int BaseBoom => 35;

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1M, this.Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var atkMove = new MoveState("ATTACK", this.AttackMove, new SingleAttackIntent(BaseAttack));
        var buffMove = new MoveState("BUFF", this.BuffMove, new BuffIntent());
        var boomMove = new MoveState("BOOM", this.BoomMove, new DeathBlowIntent(() => BaseBoom));

        atkMove.FollowUpState = buffMove;
        buffMove.FollowUpState = boomMove;

        List<MonsterState> states = [atkMove, buffMove, boomMove];

        return new MonsterMoveStateMachine(states, atkMove);
    }

    private async Task AttackMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BaseAttack).FromMonster(this)
            // .WithAttackerAnim("Attack", 0.6f).OnlyPlayAnimOnce().WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private async Task BuffMove(IReadOnlyList<Creature> targets)
    {
        var allies = this.CombatState.Enemies;
        foreach (var ally in allies)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), ally, BaseBuff, this.Creature, null);
        }
    }

    private async Task BoomMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BaseBoom).FromMonster(this)
            // .WithAttackerAnim("Attack", 0.6f).OnlyPlayAnimOnce().WithAttackerFx(null, AttackSfx)
            .WithAttackerFx(sfx: ModelDb.Monster<WaterfallGiant>().DeathSfx)
            .WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await CreatureCmd.Kill(this.Creature);
    }
}