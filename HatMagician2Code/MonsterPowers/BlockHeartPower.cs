using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.MonsterPowers;

public class BlockHeartPower : HatMagician2Power
{
    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == this.Owner && props.IsPoweredAttack() && cardSource != null && dealer is { Player: not null })
        {
            await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), dealer, this.Amount, this.Owner, null);
        }

        await base.AfterDamageReceivedLate(choiceContext, target, result, props, dealer, cardSource);
    }
}