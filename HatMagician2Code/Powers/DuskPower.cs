using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Powers;

public class DuskPower : HatMagician2Power
{
    // 每回合给予阴郁
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == this.Owner.Side)
        {
            var enemies = this.CombatState.HittableEnemies.ToList();
            foreach (var e in enemies)
            {
                await PowerCmd.Apply<GloomyPower>(choiceContext, e, this.Amount, this.Owner, null);
            }
        }

        await base.AfterSideTurnEnd(choiceContext, side, participants);
    }
}