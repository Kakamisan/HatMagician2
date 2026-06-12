using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.MonsterPowers;

public class ColorOverflowPower : HatMagician2Power
{
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == this.Owner && props.IsPoweredAttack() && cardSource != null && dealer is { Player: not null })
        {
            await HatMagician2Mgr.AddEnergy(dealer.Player, this.Amount, dealer.Player.RunState.Rng.CombatEnergyCosts.NextItem([BrandColor.Red, BrandColor.Blue, BrandColor.Yellow]));
        }

        await base.AfterDamageReceivedLate(choiceContext, target, result, props, dealer, cardSource);
    }
}