using HatMagician2.HatMagician2Code.Monsters;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.MonsterPowers;

public class ColorBleedAllPower : HatMagician2Power
{
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 被寻色者以外的生物给予印记时，进行染色
        if (power is BrandPower p && power.Owner == this.Owner && applier?.Monster is not ColorFinder)
        {
            var others = this.CombatState.Creatures.Where(c => c != this.Owner).ToList();
            foreach (var c in others)
            {
                if (c.GetPower<BrandPower>() is { } p3)
                    await PowerCmd.Remove(p3);
                await BrandPower.ApplyBrandPower(null, this.Owner, new ThrowingPlayerChoiceContext(), c, p.BaseBrandColor);
            }
        }

        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}