using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class ColorBleedPower : HatMagician2Power
{
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is BrandPower && power.Applier == this.Owner && power.Owner.Side != this.Owner.Side)
        {
            this.Flash();
            await CreatureCmd.Damage(choiceContext, this.CombatState.HittableEnemies, this.Amount, ValueProp.Unpowered, this.Owner);
        }

        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}