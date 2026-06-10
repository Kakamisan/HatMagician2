using HatMagician2.HatMagician2Code.Intents;
using HatMagician2.HatMagician2Code.MonsterPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace HatMagician2.HatMagician2Code.Monsters;

public class ColorFinder : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 999, 999);
    protected override string VanillaScene => "magi_knight";

    public bool IsAwake => !this.Creature.HasPower<FocusPaintingPower>();

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<FocusPaintingPower>(new ThrowingPlayerChoiceContext(), this.Creature, 3, null, null, true);
        await base.AfterAddedToRoom();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var focusMove = new MoveState("FOCUS", this.FocusMove, new FocusIntent());
        var attackMove = new MoveState("ATTACK", this.AttackMove, new SingleAttackIntent(20));
        var conditionalBranchState = new ConditionalBranchState("FOCUS_BRANCH");
        focusMove.FollowUpState = conditionalBranchState;
        conditionalBranchState.AddState(focusMove, () => !this.IsAwake);
        conditionalBranchState.AddState(attackMove, () => this.IsAwake);
        attackMove.FollowUpState = attackMove;

        List<MonsterState> states =
        [
            conditionalBranchState,
            focusMove,
            attackMove
        ];

        return new MonsterMoveStateMachine(states, focusMove);
    }

    private async Task FocusMove(IReadOnlyList<Creature> targets)
    {
        if (!this.IsAwake)
            TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.FOCUS.banter"), Creature, VfxColor.White);
        await Task.CompletedTask;
    }

    private async Task AttackMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(20).FromMonster(this).WithAttackerFx(null, AttackSfx).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    public async Task RemoveFocusMove()
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.FOCUS_REMOVE.banter"), Creature, VfxColor.Red);
        await Task.CompletedTask;
    }
}