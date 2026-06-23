using BaseLib.Utils.NodeFactories;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;
using HatMagician2.HatMagician2Code.Intents;
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

public class PollutedProfessorLightningBall : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 99, 86);

    public override NCreatureVisuals CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("monsters/lightning_ball.tscn".ScenePath());

    private static int BaseBuff => 3;
    private static int BaseAttack => 3;
    private static int BaseAttackCnt => 6;

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1M, this.Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var buffMove = new MoveState("BUFF", this.BuffMove, new BuffIntent());
        var debuffMove = new MoveState("BRAND", this.BrandDebuffMove, new BrandYellowIntent());
        var atkMove = new MoveState("ATTACK", this.AttackMove, new MultiAttackIntent(BaseAttack, BaseAttackCnt));

        buffMove.FollowUpState = debuffMove;
        debuffMove.FollowUpState = atkMove;
        atkMove.FollowUpState = buffMove;

        List<MonsterState> states = [buffMove, debuffMove, atkMove];

        return new MonsterMoveStateMachine(states, buffMove);
    }

    private async Task BuffMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<AgitationPower>(new ThrowingPlayerChoiceContext(), this.Creature, BaseBuff, this.Creature, null);
    }

    private async Task BrandDebuffMove(IReadOnlyList<Creature> targets)
    {
        foreach (var target in targets)
        {
            await BrandPower.ApplyBrandPower(null, this.Creature, new ThrowingPlayerChoiceContext(), target, BrandColor.Yellow);
        }
    }

    private async Task AttackMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BaseAttack).FromMonster(this).WithHitCount(BaseAttackCnt)
            // .WithAttackerAnim("Attack", 0.6f).OnlyPlayAnimOnce().WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }
}