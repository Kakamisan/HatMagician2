using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class FeebleMindPower : HatMagician2Power
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CustomCalculatedVar("AmountAdd").WithMultiplier((power, _) => ((FeebleMindPower)power).GetAmountAdd()),
        new("AmountAddBase", 0), new("AmountAddExtra", 1)
    ];

    private int GetAmountAdd() => this.Amount - 1;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is GloomyPower && power.Owner.Side != this.Owner.Side && applier == this.Owner)
        {
            await PowerCmd.Apply<FreezeStrengthPower>(choiceContext, power.Owner, amount + this.Amount - 1, applier, null);
        }

        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}