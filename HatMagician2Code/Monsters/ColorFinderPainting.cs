using BaseLib.Audio;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Intents;
using HatMagician2.HatMagician2Code.MonsterPowers;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Monsters;

public class ColorFinderPainting : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 9999999, 9999999);
    protected override string VanillaScene => "eye_with_teeth";

    protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/eye_with_teeth/eye_with_teeth_attack";

    private static decimal FirstBlock => 30;
    private static int CardNum => 6;

    // public static ModSound Bgm => new($"{MainFile.ResPath}/music/first_dance_clip.mp3", ModAudio.SoundType.Music);

    private bool IsAwake
    {
        get
        {
            var monster = (ColorFinder?)this.CombatState.Enemies.FirstOrDefault(c => c.Monster is ColorFinder)?.Monster;
            return monster == null || monster.IsAwake;
        }
    }

    public override async Task AfterAddedToRoom()
    {
        this.Creature.HpDisplay = HpDisplay.InfiniteWithoutNumbers;
        // await PowerCmd.Apply<IllusionPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1, this.Creature, null, true);
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1, this.Creature, null, true);
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), this.Creature, 999, this.Creature, null, true);
        await PowerCmd.Apply<ColorOverflowPower>(new ThrowingPlayerChoiceContext(), this.Creature, 2, this.Creature, null, true);
        // await CreatureCmd.GainBlock(this.Creature, this.FirstBlock, ValueProp.Unpowered, null);

        NRunMusicController.Instance?.StopMusic();

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
        // var colorMove = new MoveState("COLOR", this.ColorMove, new ColorIntent());
        var colorMove2 = new MoveState("COLOR2", this.ColorMove, new ColorIntent());
        var cardMove = new MoveState("COLORFUL", this.ColorfulMove, new ColorfulIntent());
        var bleedMove = new MoveState("BLEED", this.BleedMove, new BleedIntent());
        var conditionalBranchState = new ConditionalBranchState("FOCUS_BRANCH");
        focusMove.FollowUpState = conditionalBranchState;
        conditionalBranchState.AddState(focusMove, () => !this.IsAwake);
        conditionalBranchState.AddState(colorMove2, () => this.IsAwake);
        // colorMove.FollowUpState = colorMove2;
        colorMove2.FollowUpState = cardMove;
        cardMove.FollowUpState = bleedMove;
        bleedMove.FollowUpState = bleedMove;

        List<MonsterState> states =
        [
            conditionalBranchState, focusMove,
            colorMove2, cardMove, bleedMove,
        ];

        return new MonsterMoveStateMachine(states, focusMove);
    }

    private async Task FocusMove(IReadOnlyList<Creature> targets)
    {
        // TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER_PAINTING.moves.FOCUS.banter"), Creature, VfxColor.White);
        await Task.CompletedTask;
    }

    private async Task ColorMove(IReadOnlyList<Creature> targets)
    {
        //TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER_PAINTING.moves.COLOR.banter"), Creature, VfxColor.White);
        SfxCmd.Play(this.AttackSfx);
        await CreatureCmd.TriggerAnim(this.Creature, "Attack", 0.7f);
        foreach (var target in targets)
        {
            if (target is { Player: not null, IsAlive: true })
                await HatMagician2Mgr.AddEnergy(target.Player, 2, BrandColor.Rainbow);
        }
    }

    private async Task ColorfulMove(IReadOnlyList<Creature> targets)
    {
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER_PAINTING.moves.COLORFUL.banter"), Creature, VfxColor.White);
        SfxCmd.Play(this.AttackSfx);
        await CreatureCmd.TriggerAnim(this.Creature, "Attack", 0.7f);
        // ModAudio.PlaySound(Bgm, -3);
        Hat2Audio.ColorFinderBgm.Play();
        await CreatureCmd.TriggerAnim(this.Creature, "Attack", 1.4f);
        foreach (var target in targets)
        {
            if (target is { Player: not null, IsAlive: true })
            {
                var card = this.CombatState.CreateCard<ColorStatus>(target.Player);
                for (int i = 0; i < CardNum; i++)
                {
                    CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, null, CardPilePosition.Random));
                    card = this.CombatState.CreateCard<ColorStatus>(target.Player);
                }
            }
        }

        await PowerCmd.Apply<ColorBleedAllPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1, this.Creature, null, true);
    }

    private async Task BleedMove(IReadOnlyList<Creature> targets)
    {
        // TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER_PAINTING.moves.FOCUS.banter"), Creature, VfxColor.White);
        await Task.CompletedTask;
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        AnimState initialState = new AnimState("idle_loop", true);
        AnimState state1 = new AnimState("attack");
        AnimState state2 = new AnimState("die");
        state1.NextState = initialState;
        CreatureAnimator animator = new CreatureAnimator(initialState, controller);
        animator.AddAnyState("Attack", state1);
        animator.AddAnyState("Dead", state2, (Func<bool>)(() => !this.CombatState.GetTeammatesOf(this.Creature).Any(t => t is { IsPrimaryEnemy: true, IsAlive: true })));
        return animator;
    }
}