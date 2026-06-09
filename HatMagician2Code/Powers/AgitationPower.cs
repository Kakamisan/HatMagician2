using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class AgitationPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public bool TryModifyChainDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, out decimal modifiedDamage)
    {
        if (dealer == this.Owner)
        {
            modifiedDamage = amount + this.Amount;
            return true;
        }

        modifiedDamage = amount;
        return false;
    }

    public override bool HasChangeBrandValEffect => true;

    public bool TryModifyEvokeValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandYellowPower && power.Owner.Side != this.Owner.Side)
        {
            modifiedVal = originVal + this.Amount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyPassiveValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandYellowPower or BrandOrangePower && power.Owner.Side != this.Owner.Side)
        {
            modifiedVal = originVal + this.Amount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyFusionValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandOrangePower && power.Owner.Side != this.Owner.Side)
        {
            modifiedVal = originVal + this.Amount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }
}