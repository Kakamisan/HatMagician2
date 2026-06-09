using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace HatMagician2.HatMagician2Code.Monsters;

public abstract class ColorFinderPainting : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 9999999, 9999999);
    protected override string VanillaScene => "scroll_of_biting";

    public override Task AfterAddedToRoom()
    {
        return base.AfterAddedToRoom();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        throw new NotImplementedException();
    }
}