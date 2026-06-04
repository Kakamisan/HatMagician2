using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Character;

public interface IHatMagician2AbstractModel
{
    public bool TryModifyBrandColorCost(HatMagician2Card card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        return false;
    }

    // 加倍 加法叠加
    public bool TryModifyEvokeValMultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyEvokeValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    // 加倍 加法叠加
    public bool TryModifyEvokeVal2MultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyEvokeVal2Additive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    // 加倍 加法叠加
    public bool TryModifyPassiveValMultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyPassiveValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    // 加倍 加法叠加
    public bool TryModifyFusionValMultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyFusionValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    public Task AfterAddEnergy(Player player, int amount, BrandColor color)
    {
        return Task.CompletedTask;
    }

    public Task AfterSpendEnergy(Player player, int amount, BrandColor color)
    {
        return Task.CompletedTask;
    }

    public bool TryModifyChainDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, out decimal modifiedDamage)
    {
        modifiedDamage = amount;
        return false;
    }

    public Task AfterGloomyDamage(Creature target, decimal damage, Creature? dealer)
    {
        return Task.CompletedTask;
    }

    public Task AfterBrandPowerEvoke(BrandPower power)
    {
        return Task.CompletedTask;
    }

    // 攻击倍率 加法叠加
    public int TryModifyMultiDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel cardSource)
    {
        return 0;
    }
}