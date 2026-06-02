using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class BluePainting : HatMagician2Relic, IHatMagician2AbstractModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public bool TryModifyEvokeValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandBluePower)
        {
            modifiedVal = originVal + 1;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyPassiveValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandBluePower)
        {
            modifiedVal = originVal + 1;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyFusionValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandWhitePower or BrandPurplePower)
        {
            modifiedVal = originVal + 1;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }
}