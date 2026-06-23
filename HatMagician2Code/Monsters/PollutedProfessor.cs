using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace HatMagician2.HatMagician2Code.Monsters;

public class PollutedProfessor : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 657, 535);
    protected override string VanillaScene => "queen";
    protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/queen/queen_arms_attack";
    protected override string CastSfx => "event:/sfx/enemy/enemy_attacks/queen/queen_cast";
    public override string DeathSfx => "event:/sfx/enemy/enemy_attacks/queen/queen_die";

    private static int BaseAttack1 => 22;
    private static int BaseAttackCnt1 => 1;
    private static int BaseAttack2 => 5;
    private static int BaseAttackCnt2 => 4;
    private static int BaseSummonCnt => 2; // 2个召唤物去世时 下回合召唤2个召唤物

    private string _lastSummon = "ice";
    private int _cycleMoveCnt;

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), this.Creature, 3, null, null, true);
        await base.AfterAddedToRoom();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var summonAllMove = new MoveState("SUMMON_ALL", this.SummonAll, new SummonIntent());
        var summonOneMove = new MoveState("SUMMON_ONE", this.SummonOne, new SummonIntent());
        var debuffMove = new MoveState("DEBUFF", this.DebuffMove, new DebuffIntent(), new CardDebuffIntent());
        var atk1Move = new MoveState("ATTACK1", this.AttackMove1, new SingleAttackIntent(BaseAttack1));
        var atk2Move = new MoveState("ATTACK2", this.AttackMove2, new MultiAttackIntent(BaseAttack2, BaseAttackCnt2));

        var summonBranchState = new ConditionalBranchState("SUMMON_BRANCH");
        var cycleBranchState = new ConditionalBranchState("CYCLE_BRANCH");
        summonAllMove.FollowUpState = summonBranchState;
        summonOneMove.FollowUpState = summonBranchState;
        debuffMove.FollowUpState = summonBranchState;
        atk1Move.FollowUpState = summonBranchState;
        atk2Move.FollowUpState = summonBranchState;

        summonBranchState.AddState(summonOneMove, this.IsAnyCntSummonDead);
        summonBranchState.AddState(cycleBranchState, () => !this.IsAnyCntSummonDead());

        cycleBranchState.AddState(debuffMove, () => this._cycleMoveCnt % 3 == 0);
        cycleBranchState.AddState(atk1Move, () => this._cycleMoveCnt % 3 == 1);
        cycleBranchState.AddState(atk2Move, () => this._cycleMoveCnt % 3 == 2);

        List<MonsterState> states =
        [
            summonAllMove, summonOneMove, summonBranchState, cycleBranchState,
            atk1Move, atk2Move, debuffMove
        ];

        return new MonsterMoveStateMachine(states, summonAllMove);
    }

    private async Task AttackMove1(IReadOnlyList<Creature> targets)
    {
        this._cycleMoveCnt += 1;
        await DamageCmd.Attack(BaseAttack1).WithHitCount(BaseAttackCnt1).FromMonster(this).WithAttackerAnim("Attack", 0.6f).OnlyPlayAnimOnce().WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private async Task AttackMove2(IReadOnlyList<Creature> targets)
    {
        this._cycleMoveCnt += 1;
        await DamageCmd.Attack(BaseAttack2).WithHitCount(BaseAttackCnt2).FromMonster(this).WithAttackerAnim("Attack", 0.6f).OnlyPlayAnimOnce().WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt").Execute(null);
    }

    private async Task DebuffMove(IReadOnlyList<Creature> targets)
    {
        this._cycleMoveCnt += 1;
        await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), targets, 3, this.Creature, null);
        // await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, 3, this.Creature, null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, 3, this.Creature, null);
        foreach (var target in targets)
        {
            if (target is { Player: not null, IsAlive: true })
            {
                var card = this.CombatState.CreateCard<ColorPollutionStatus>(target.Player);
                card.DynamicColor = target.Player.RunState.Rng.CombatEnergyCosts.NextItem([BrandColor.Red, BrandColor.Blue, BrandColor.Yellow]);
                card.TargetOwner = this.Creature;
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, null, CardPilePosition.Random));
            }
        }
    }

    private async Task SummonAll(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(CastSfx);
        await CreatureCmd.TriggerAnim(this.Creature, "Cast", 0.5f);
        if (this.CombatState.IsLiveCombat())
        {
            await CreatureCmd.Add<PollutedProfessorFireBall>(this.CombatState, "fire");
            await CreatureCmd.Add<PollutedProfessorLightningBall>(this.CombatState, "lightning");
            await CreatureCmd.Add<PollutedProfessorIceBall>(this.CombatState, "ice");
        }
    }

    private async Task SummonOne(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(CastSfx);
        await CreatureCmd.TriggerAnim(this.Creature, "Cast", 0.5f);
        if (this.CombatState.IsLiveCombat())
        {
            // 检查召唤的顺序 优先上次召唤的球的下一个球
            List<string> list = this._lastSummon switch
            {
                "ice" => ["fire", "lightning", "ice"],
                "fire" => ["lightning", "ice", "fire"],
                _ => ["ice", "fire", "lightning"]
            };
            var summonCnt = 0;
            foreach (var type in list.Where(this.IsSummonDead))
            {
                await this.SummonByType(type);
                this._lastSummon = type;
                summonCnt += 1;
                if (summonCnt >= BaseSummonCnt) break;
            }
        }
    }

    // 召唤物是否去世了
    private bool IsSummonDead(string type)
    {
        return !this.CombatState.Enemies.Any(e => IsEnemySummonType(e, type));
    }

    private static bool IsEnemySummonType(Creature creature, string type)
    {
        var flag = type switch
        {
            "fire" => creature.Monster is PollutedProfessorFireBall,
            "lightning" => creature.Monster is PollutedProfessorLightningBall,
            _ => creature.Monster is PollutedProfessorIceBall
        };
        return flag;
    }

    // 任意2个召唤物去世 下回合意图变召唤
    private bool IsAnyCntSummonDead()
    {
        List<string> list = ["fire", "lightning", "ice"];
        return list.Count(this.IsSummonDead) >= BaseSummonCnt;
    }

    private async Task SummonByType(string type)
    {
        _ = type switch
        {
            "fire" => await CreatureCmd.Add<PollutedProfessorFireBall>(this.CombatState, "fire"),
            "lightning" => await CreatureCmd.Add<PollutedProfessorLightningBall>(this.CombatState, "lightning"),
            _ => await CreatureCmd.Add<PollutedProfessorIceBall>(this.CombatState, "ice")
        };
        await Task.CompletedTask;
    }
}