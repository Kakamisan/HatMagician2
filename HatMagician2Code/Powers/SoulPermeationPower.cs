using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Logging;

namespace HatMagician2.HatMagician2Code.Powers;

public class SoulPermeationPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool HasChangeBrandValEffect => true;

    public bool TryModifyEvokeVal(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandRedPower)
        {
            modifiedVal = originVal + this.Amount - 1;
        }
        else
        {
            modifiedVal = originVal * this.Amount;
        }

        return true;
    }

    public bool TryModifyPassiveVal(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal * this.Amount;

        return true;
    }
}