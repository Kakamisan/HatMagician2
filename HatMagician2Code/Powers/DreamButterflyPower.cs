using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HatMagician2.HatMagician2Code.Powers;

public class DreamButterflyPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public static int AddCostThisTurn(Player player)
    {
        DreamButterflyPower? power = (DreamButterflyPower?)player.Creature.Powers.FirstOrDefault(p => p is DreamButterflyPower);
        if (power != null)
            return -power.Amount;
        return 0;
    }
}