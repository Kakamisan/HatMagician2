using HatMagician2.HatMagician2Code.Monsters;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.MonsterPowers;

public class FocusPaintingPower : HatMagician2Power
{
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if ((target == this.Owner || target.Monster is ColorFinderPainting) && result.UnblockedDamage > 0)
        {
            await ((ColorFinder)this.Owner.Monster!).RemoveFocusMove();
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(this.Owner))
            return;
        if (this.Amount == 1) await ((ColorFinder)this.Owner.Monster!).RemoveFocusMove();
        await PowerCmd.Decrement(this);
    }
}