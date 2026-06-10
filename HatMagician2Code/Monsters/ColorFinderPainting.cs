using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Intents;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace HatMagician2.HatMagician2Code.Monsters;

public class ColorFinderPainting : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 9999999, 9999999);
    protected override string VanillaScene => "parafright";

    public bool IsAwake
    {
        get
        {
            var monster = (ColorFinder?)this.CombatState.Enemies.FirstOrDefault(c => c.Monster is ColorFinder)?.Monster;
            return monster == null || monster.IsAwake;
        }
    }

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), this.Creature, 999, this.Creature, null, true);
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), this.Creature, 1, this.Creature, null, true);
        await base.AfterAddedToRoom();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var focusMove = new MoveState("FOCUS", this.FocusMove, new FocusIntent());
        var colorMove = new MoveState("COLOR", this.ColorMove, new ColorIntent());
        var colorMove2 = new MoveState("COLOR2", this.ColorMove, new ColorIntent());
        var cardMove = new MoveState("COLORFUL", this.ColorfulMove, new ColorfulIntent());
        var bleedMove = new MoveState("BLEED", this.BleedMove, new BleedIntent());
        var conditionalBranchState = new ConditionalBranchState("FOCUS_BRANCH");
        focusMove.FollowUpState = conditionalBranchState;
        conditionalBranchState.AddState(focusMove, () => !this.IsAwake);
        conditionalBranchState.AddState(colorMove, () => this.IsAwake);
        colorMove.FollowUpState = colorMove2;
        colorMove2.FollowUpState = cardMove;
        cardMove.FollowUpState = bleedMove;
        bleedMove.FollowUpState = bleedMove;

        List<MonsterState> states =
        [
            conditionalBranchState, focusMove, colorMove,
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
        TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER_PAINTING.moves.COLOR.banter"), Creature, VfxColor.White);
        foreach (var target in targets)
        {
            if (target is { Player: not null, IsAlive: true })
                await HatMagician2Mgr.AddEnergy(target.Player, 1, BrandColor.Rainbow);
        }
    }

    private async Task ColorfulMove(IReadOnlyList<Creature> targets)
    {
        foreach (var target in targets)
        {
            if (target is { Player: not null, IsAlive: true })
            {
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(target.Player), PileType.Draw, null, CardPilePosition.Random));
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(target.Player), PileType.Draw, null, CardPilePosition.Random));
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(target.Player), PileType.Draw, null, CardPilePosition.Random));
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(target.Player), PileType.Draw, null, CardPilePosition.Random));
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(target.Player), PileType.Draw, null, CardPilePosition.Random));
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(target.Player), PileType.Draw, null, CardPilePosition.Random));
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(target.Player), PileType.Draw, null, CardPilePosition.Random));
            }
        }
    }

    private async Task BleedMove(IReadOnlyList<Creature> targets)
    {
        // TalkCmd.Play(L10NMonsterLookup("HATMAGICIAN2-COLOR_FINDER_PAINTING.moves.FOCUS.banter"), Creature, VfxColor.White);
        await Task.CompletedTask;
    }
}