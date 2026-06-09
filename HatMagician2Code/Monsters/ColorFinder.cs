using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace HatMagician2.HatMagician2Code.Monsters;

public abstract class ColorFinder : HatMagician2Monster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 999, 999);
    protected override string VanillaScene => "magi_knight";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        throw new NotImplementedException();
    }
}