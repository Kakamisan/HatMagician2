using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.MonsterPowers;

public class ColorLeechPower : HatMagician2Power
{
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target.Player is { } player && dealer == this.Owner)
        {
            await HatMagician2Mgr.SpendEnergy(player, this.Amount, BrandColor.Any, false);
        }

        await base.AfterDamageReceivedLate(choiceContext, target, result, props, dealer, cardSource);
    }
}