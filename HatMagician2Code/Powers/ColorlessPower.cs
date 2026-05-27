using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class ColorlessPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = Math.Max(0, originalCost - this.Amount);
        return true;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != this.Owner.Side)
            return;
        //this.Flash();
        await PowerCmd.Remove(this);
    }

    // 拥有幽寂时不能继续获得虹光/幽暗
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        if (target == this.Owner && canonicalPower is CollectPower)
        {
            modifiedAmount = 0;
            return true;
        }

        return base.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);
    }
}