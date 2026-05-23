using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class CollectPower : HatMagician2Power
{
    public int LightAmount => this.Owner.GetPower<CollectLightPower>()?.Amount ?? 0;
    public int DarkAmount => this.Owner.GetPower<CollectDarkPower>()?.Amount ?? 0;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // Log.Info("[   Hat2   ]AfterApplied: " + this.LightAmount + " + " + this.DarkAmount);
        await this.TryTransTo();
        await base.AfterApplied(applier, cardSource);
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        if (canonicalPower is CollectPower)
        {
            modifiedAmount = Math.Max(0, Math.Min(amount, 7 - this.LightAmount - this.DarkAmount));
            return true;
        }

        return base.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        // Log.Info("[   Hat2   ]AfterModifyingPowerAmountReceived: " + this.LightAmount + " + " + this.DarkAmount);
        await this.TryTransTo();
        await base.AfterModifyingPowerAmountReceived(power);
    }

    // 进入虹彩/幽寂
    private async Task TryTransTo()
    {
        if (this.LightAmount + this.DarkAmount >= 7)
        {
            if (this.LightAmount > this.DarkAmount)
            {
                await PowerCmd.Apply<ColorfulPower>(new ThrowingPlayerChoiceContext(), this.Owner, 1, this.Owner, null);
            }
            else
            {
                await PowerCmd.Apply<ColorlessPower>(new ThrowingPlayerChoiceContext(), this.Owner, 1, this.Owner, null);
            }

            // 移除所有虹光/幽暗
            var powers = this.Owner.Powers.Where(p => p is CollectPower).ToList();
            foreach (var power in powers)
            {
                await PowerCmd.Remove(power);
            }
        }
    }
}