using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Logging;

namespace HatMagician2.HatMagician2Code.Powers;

public class SoulPermeationPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public override bool HasChangeBrandValEffect => true;

    public bool TryModifyEvokeValMulti(BrandPower power, decimal originVal, out decimal modifiedVal)
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

    public bool TryModifyEvokeVal2Multi(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal * this.Amount;
        return true;
    }

    public bool TryModifyPassiveValMulti(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal * this.Amount;

        return true;
    }

    public bool TryModifyFusionValMulti(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal * this.Amount;

        return true;
    }
}