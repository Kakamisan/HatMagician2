using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HatMagician2.HatMagician2Code.Powers;

public class DrawCardsNextTurnPatchPower : HatMagician2Power
{
    public override PowerType Type => this.Amount > 0 ? PowerType.Buff : PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool AllowNegative => true;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player != this.Owner.Player || this.AmountOnTurnStart == 0 ? count : count + this.Amount;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(this.Owner) || this.AmountOnTurnStart == 0)
            return;
        await PowerCmd.Remove(this);
    }
}