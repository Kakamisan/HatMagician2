using HatMagician2.HatMagician2Code.Cards;

namespace HatMagician2.HatMagician2Code.Character;

public interface IHatMagician2AbstractModel
{
    public bool TryModifyBrandColorCost(HatMagician2Card card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        return false;
    }
}