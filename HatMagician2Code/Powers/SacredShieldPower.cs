using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class SacredShieldPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public async Task AfterSpendEnergy(Player player, int amount, BrandColor color)
    {
        if (color == BrandColor.White && amount > 0)
        {
            await CreatureCmd.GainBlock(this.Owner, this.Amount * amount, ValueProp.Unpowered, null);
        }
    }

    public async Task AfterAddEnergy(Player player, int amount, BrandColor color)
    {
        if (color == BrandColor.White && amount > 0)
        {
            await CreatureCmd.GainBlock(this.Owner, this.Amount * amount, ValueProp.Unpowered, null);
        }
    }
}