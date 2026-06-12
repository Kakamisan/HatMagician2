using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Intents;
using HatMagician2.HatMagician2Code.MonsterPowers;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Monsters;

public class ColorFinder : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 999, 999);
    protected override string VanillaScene => "magi_knight";
    public override string HurtSfx => "event:/sfx/enemy/enemy_attacks/magi_knight/magi_knight_hurt";
    protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/magi_knight/magi_knight_attack_ram";

    public bool IsAwake => !this.Creature.HasPower<FocusPaintingPower>();

    private Creature? Painting => this.CombatState.GetCreaturesOnSide(CombatSide.Enemy).FirstOrDefault(c => c.Monster is ColorFinderPainting);

    private bool PaintingRainbow => this.Painting?.GetPower<BrandPower>() is BrandRainbowPower;

    private static decimal FirstBlock => 30;
    private static int BaseAttack1 => 30;
    private static int BasePower1 => 3;
    private static int BaseAttack2 => 11;
    private static int BaseAttack2Repeat => 3;
    private static int BaseAttack3 => 8;
    private static int BaseAttack3Repeat => 3;
    private static int BasePower2 => 3;

    private int _brandMoveCnt;

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<FocusPaintingPower>(new ThrowingPlayerChoiceContext(), this.Creature, 3, null, null, true);
        await PowerCmd.Apply<ColorLeechPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1, null, null, true);
        await PowerCmd.Apply<BlockHeartPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1, null, null, true);
        // await CreatureCmd.GainBlock(this.Creature, FirstBlock, ValueProp.Unpowered, null);
        await base.AfterAddedToRoom();
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player && combatState.RoundNumber == 1)
            await CreatureCmd.GainBlock(this.Creature, FirstBlock, ValueProp.Unpowered, null);
        await base.BeforeSideTurnStart(choiceContext, side, participants, combatState);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var focusMove = new MoveState("FOCUS", this.FocusMove, new FocusIntent());
        var attackMove1 = new MoveState("ATTACK1", this.AttackMove1, new SingleAttackIntent(BaseAttack1), new BuffIntent());
        var attackMove2 = new MoveState("ATTACK2", this.AttackMove2, new MultiAttackIntent(BaseAttack2, BaseAttack2Repeat), new BrandYellowIntent());
        var pollutionMove = new MoveState("POLLUTION", this.PollutionMove, new DebuffIntent());
        var brandMove1 = new MoveState("BRAND1", this.BrandMove, new MultiAttackIntent(BaseAttack3, BaseAttack3Repeat), new BrandRedIntent(),
            new ColorFinderPaintIntent(), new HateRainbowIntent());
        var brandMove2 = new MoveState("BRAND2", this.BrandMove, new MultiAttackIntent(BaseAttack3, BaseAttack3Repeat), new BrandBlueIntent(),
            new ColorFinderPaintIntent(), new HateRainbowIntent());
        var brandMove3 = new MoveState("BRAND3", this.BrandMove, new MultiAttackIntent(BaseAttack3, BaseAttack3Repeat), new BrandYellowIntent(),
            new ColorFinderPaintIntent(), new HateRainbowIntent());
        var clearMove = new MoveState("CLEAR", this.ClearMove, new UnknownIntent());

        var conditionalBranchState = new ConditionalBranchState("FOCUS_BRANCH");
        focusMove.FollowUpState = conditionalBranchState;
        conditionalBranchState.AddState(focusMove, () => !this.IsAwake);
        conditionalBranchState.AddState(attackMove1, () => this.IsAwake);
        attackMove1.FollowUpState = attackMove2;
        attackMove2.FollowUpState = pollutionMove;
        pollutionMove.FollowUpState = brandMove1;
        var conditionalBranchState2 = new ConditionalBranchState("BRAND_BRANCH");
        brandMove1.FollowUpState = conditionalBranchState2;
        brandMove2.FollowUpState = conditionalBranchState2;
        brandMove3.FollowUpState = conditionalBranchState2;
        conditionalBranchState2.AddState(brandMove1, () => !this.PaintingRainbow && this._brandMoveCnt == 0);
        conditionalBranchState2.AddState(brandMove2, () => !this.PaintingRainbow && this._brandMoveCnt == 1);
        conditionalBranchState2.AddState(brandMove3, () => !this.PaintingRainbow && this._brandMoveCnt == 2);
        conditionalBranchState2.AddState(clearMove, () => this.PaintingRainbow);

        var conditionalBranchState3 = new ConditionalBranchState("BRAND_BRANCH2");
        clearMove.FollowUpState = conditionalBranchState3;
        conditionalBranchState3.AddState(brandMove1, () => this._brandMoveCnt == 0);
        conditionalBranchState3.AddState(brandMove2, () => this._brandMoveCnt == 1);
        conditionalBranchState3.AddState(brandMove3, () => this._brandMoveCnt == 2);

        List<MonsterState> states =
        [
            conditionalBranchState, focusMove, attackMove1,
            attackMove2, pollutionMove, brandMove1, brandMove2,
            brandMove3, clearMove, conditionalBranchState2,
            conditionalBranchState3,
        ];

        return new MonsterMoveStateMachine(states, focusMove);
    }

    private async Task FocusMove(IReadOnlyList<Creature> targets)
    {
        if (!this.IsAwake)
            TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.FOCUS.banter"), Creature, VfxColor.White);
        await Task.CompletedTask;
    }

    public async Task RemoveFocusMove()
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.FOCUS_REMOVE.banter"), Creature, VfxColor.Red);
        await Task.CompletedTask;
    }

    private async Task AttackMove1(IReadOnlyList<Creature> targets)
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.ATTACK.banter"), Creature, VfxColor.White);
        await DamageCmd.Attack(BaseAttack1).FromMonster(this).WithAttackerAnim("RamAttack", 1.2f).WithAttackerFx(null, AttackSfx).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<AgitationPower>(new ThrowingPlayerChoiceContext(), this.Creature, BasePower1, this.Creature, null);
    }

    private async Task AttackMove2(IReadOnlyList<Creature> targets)
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.ATTACK2.banter"), Creature, VfxColor.White);
        await CreatureCmd.TriggerAnim(this.Creature, "RamAttack", 1.2f);
        await DamageCmd.Attack(BaseAttack2).FromMonster(this).WithHitCount(BaseAttack2Repeat).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        foreach (var target in targets)
        {
            await BrandPower.ApplyBrandPower(null, this.Creature, new ThrowingPlayerChoiceContext(), target, BrandColor.Yellow);
        }
    }

    private async Task PollutionMove(IReadOnlyList<Creature> targets)
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.POLLUTION.banter"), Creature, VfxColor.White);
        await PowerCmd.Remove(this.Creature.GetPower<BlockHeartPower>());
        // 污染能力
        foreach (Creature creature in targets)
        {
            PollutionPower mutable = (PollutionPower)ModelDb.Power<PollutionPower>().ToMutable();
            mutable.Target = creature;
            await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), mutable, this.Creature, PollutionPower.BaseCardsLeft, this.Creature, null);
        }
    }

    private async Task BrandMove(IReadOnlyList<Creature> targets)
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.BRAND.banter"), Creature, VfxColor.White);
        await CreatureCmd.TriggerAnim(this.Creature, "RamAttack", 1.2f);
        await DamageCmd.Attack(BaseAttack3).FromMonster(this).WithHitCount(BaseAttack3Repeat).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), this.Creature, BasePower2, this.Creature, null);
        var color = this._brandMoveCnt switch
        {
            0 => BrandColor.Red,
            1 => BrandColor.Blue,
            _ => BrandColor.Yellow
        };

        this._brandMoveCnt++;
        if (this._brandMoveCnt > 2) this._brandMoveCnt = 0;

        foreach (var target in targets)
        {
            await BrandPower.ApplyBrandPower(null, this.Creature, new ThrowingPlayerChoiceContext(), target, color);
        }

        if (this.Painting is not null)
        {
            await BrandPower.ApplyBrandPower(null, this.Creature, new ThrowingPlayerChoiceContext(), this.Painting, color);
            if (this.PaintingRainbow)
                TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.CLEAR.banter"), Creature, VfxColor.White);
        }
    }

    private async Task ClearMove(IReadOnlyList<Creature> targets)
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER.moves.CLEAR.banter2"), Creature, VfxColor.White);
        await PowerCmd.Remove(this.Creature.GetPower<StrengthPower>());
        foreach (var target in this.CombatState.Creatures)
        {
            await PowerCmd.Remove(target.GetPower<BrandPower>());
        }
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        AnimState initialState = new AnimState("idle_loop", true);
        AnimState state1 = new AnimState("attack_bomb");
        AnimState state2 = new AnimState("attack_ram");
        AnimState state3 = new AnimState("cast_shield");
        AnimState state4 = new AnimState("hurt");
        AnimState state5 = new AnimState("die");
        state1.NextState = initialState;
        state3.NextState = initialState;
        state2.NextState = initialState;
        state4.NextState = initialState;
        CreatureAnimator animator = new CreatureAnimator(initialState, controller);
        animator.AddAnyState("Dead", state5);
        animator.AddAnyState("Hit", state4);
        animator.AddAnyState("Cast", state1);
        animator.AddAnyState("BombCast", state1);
        animator.AddAnyState("RamAttack", state2);
        animator.AddAnyState("ShieldAttack", state3);
        return animator;
    }
}