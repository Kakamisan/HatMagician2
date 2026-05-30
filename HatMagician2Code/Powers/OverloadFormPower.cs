using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class OverloadFormPower : HatMagician2Power, IHatMagician2AbstractModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MultiDamagePower>()];

    public bool TryModifyEvokeValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandRedPower)
        {
            modifiedVal = originVal + this.Amount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public async Task AfterBrandPowerEvoke(BrandPower power)
    {
        if (power is BrandRedPower) return;
        await PowerCmd.Apply<MultiDamagePower>(new ThrowingPlayerChoiceContext(), power.Owner, this.Amount, power.Applier, null);
    }

    // 如果敌人已有灼痕 那么增加倍数=Amount 如果敌人没灼痕 那么增加倍数=Amount-1
    // 这里和火焰印记一样 只是预览时增加倍数
    public int TryModifyMultiDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel cardSource)
    {
        if (cardSource is HatMagician2Card card && card.IsEvokeCard() && !card.IsBrandAppliedBeforeAttack &&
            target?.Powers.FirstOrDefault(p => p is BrandPower and not BrandRedPower) is BrandPower)
        {
            return MultiDamagePower.GetAmount(target) > 0 ? this.Amount : this.Amount - 1;
        }

        return 0;
    }
}