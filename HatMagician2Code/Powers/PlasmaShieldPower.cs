using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class PlasmaShieldPower : HatMagician2Power
{
    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (target != this.Owner || dealer == null || !props.IsPoweredAttack())
            return;
        this.Flash();
        await BrandPower.ApplyBrandPower(null, this.Owner, new ThrowingPlayerChoiceContext(), dealer, BrandColor.Yellow);
        await PowerCmd.Decrement(this);
        await base.AfterDamageReceivedLate(choiceContext, target, result, props, dealer, cardSource);
    }
}