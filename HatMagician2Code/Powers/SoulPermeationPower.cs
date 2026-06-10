using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class SoulPermeationPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public override bool HasChangeBrandValEffect => true;

    public bool TryModifyEvokeValMultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power.Owner.Side == this.Owner.Side)
        {
            modifiedVal = originVal;
            return false;
        }

        modifiedVal = originVal + this.Amount - 1;
        return true;
    }

    public bool TryModifyEvokeVal2MultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power.Owner.Side == this.Owner.Side)
        {
            modifiedVal = originVal;
            return false;
        }

        modifiedVal = originVal + this.Amount - 1;
        return true;
    }

    public bool TryModifyPassiveValMultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power.Owner.Side == this.Owner.Side)
        {
            modifiedVal = originVal;
            return false;
        }

        modifiedVal = originVal + this.Amount - 1;
        return true;
    }

    public bool TryModifyFusionValMultiAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power.Owner.Side == this.Owner.Side)
        {
            modifiedVal = originVal;
            return false;
        }

        modifiedVal = originVal + this.Amount - 1;
        return true;
    }

    // 战斗中生成印记牌时添加侵蚀
    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card is HatMagician2Card { HasBrandApply: true } && !card.Keywords.Contains(HatMagician2Keywords.Erosion) && card.Owner == this.Owner.Player)
        {
            card.AddKeyword(HatMagician2Keywords.Erosion);
        }

        return base.AfterCardGeneratedForCombat(card, creator);
    }
}