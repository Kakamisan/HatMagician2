using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace HatMagician2.HatMagician2Code.Powers;

public class OverloadFormPower : HatMagician2Power, IHatMagician2AbstractModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MultiDamagePower>()];

    public bool TryModifyEvokeValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandRedPower)
        {
            modifiedVal = originVal + this.Amount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public async Task AfterBrandPowerEvoke(BrandPower power)
    {
        if (power is BrandRedPower) return;
        if (power.Owner.HasPower<MultiDamagePower>())
        {
            await PowerCmd.Apply<MultiDamagePower>(new ThrowingPlayerChoiceContext(), power.Owner, this.Amount, power.Applier, null);
        }
        else
        {
            await PowerCmd.Apply<MultiDamagePower>(new ThrowingPlayerChoiceContext(), power.Owner, this.Amount + 1, power.Applier, null);
        }
    }
}