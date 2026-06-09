using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

// 灼痕 - 下次受到伤害*N 火焰印记附属能力
public class MultiDamagePower : HatMagician2Power, IHatMagician2AbstractModel
{
    public override PowerType Type => PowerType.Debuff;
    public override bool FakeDebuff => true;

    private bool _ready2Remove;

    // 受到攻击后删除能力
    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (IsTriggerMulti(cardSource) && props.IsPoweredAttack() && this.Owner == target && this.Amount > 1)
        {
            // this.Flash();
            this._ready2Remove = true;
            // PowerCmd.Remove(this);
        }

        return base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }

    public override Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this._ready2Remove)
        {
            this.Flash();
            PowerCmd.Remove(this);
        }

        return base.AfterCardPlayedLate(choiceContext, cardPlay);
    }

    // 是否触发倍伤 攻击牌即触发 （后续优化 伤害大于0才触发？暂时没思路）
    public static bool IsTriggerMulti(CardModel? cardSource, decimal amount = 1) => cardSource is { Type: CardType.Attack } && amount > 0;

    public int TryModifyMultiDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel cardSource)
    {
        return target == this.Owner && props.IsPoweredAttack() ? this.Amount - 1 : 0;
    }

    // 获取某只怪的灼痕层数
    public static int GetAmount(Creature? owner)
    {
        if (owner != null) return owner.Powers.FirstOrDefault(p => p is MultiDamagePower)?.Amount ?? 0;
        return 0;
    }

    // 非职业卡 走通用乘算
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (cardSource is HatMagician2Card) return 1;
        return IsTriggerMulti(cardSource) && props.IsPoweredAttack() && this.Owner == target ? this.Amount : 1;
    }
}