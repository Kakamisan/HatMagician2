using MegaCrit.Sts2.Core.Entities.Players;

namespace HatMagician2.HatMagician2Code.Powers;

public class DreamButterflyPower : HatMagician2Power
{
    public static int AddCostThisTurn(Player player)
    {
        DreamButterflyPower? power = player.Creature.GetPower<DreamButterflyPower>();
        if (power != null)
            return -power.Amount;
        return 0;
    }

    public static int AddBrandColorCostThisTurn(Player player)
    {
        return AddCostThisTurn(player);
    }
}