using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class FeebleMindPower : HatMagician2Power
{
    public override PowerStackType StackType => this.Amount > 1 ? PowerStackType.Counter : PowerStackType.Single;
    // public override PowerStackType StackType => PowerStackType.Single;
    // public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    // protected override IEnumerable<DynamicVar> CanonicalVars =>
    // [
    //     new CustomCalculatedVar("AmountAdd").WithMultiplier((power, _) => ((FeebleMindPower)power).GetAmountAdd()),
    //     new("AmountAddBase", 0), new("AmountAddExtra", 1)
    // ];

    // private int GetAmountAdd() => this.Amount - 1;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is GloomyPower && power.Owner.Side != this.Owner.Side && applier == this.Owner)
        {
            this.Flash();
            for (int i = 0; i < this.Amount; i++)
            {
                await PowerCmd.Apply<FreezeStrengthPower>(choiceContext, power.Owner, amount, applier, null);
            }
        }

        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}