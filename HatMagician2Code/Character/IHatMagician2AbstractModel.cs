using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Players;

namespace HatMagician2.HatMagician2Code.Character;

public interface IHatMagician2AbstractModel
{
    public bool TryModifyBrandColorCost(HatMagician2Card card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        return false;
    }

    public bool TryModifyEvokeVal(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyPassiveVal(BrandPower power, decimal originVal, out decimal modifiedVal)
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
}