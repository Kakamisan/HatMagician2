using HatMagician2.HatMagician2Code.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class MultiDamagePower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private decimal Multi => this.Amount;

    // protected override IEnumerable<DynamicVar> CanonicalVars => [new("Multi", this.Multi)];

    // 被攻击牌攻击时 伤害x倍数
    // 如果已经设置了card.NextPlayMulti则不应用加成
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // var val = base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
        if (IsTriggerMulti(cardSource) && this.Owner == target &&
            !BrandRedPower.WillTriggerMultiDamage(cardSource, target) &&
            cardSource is not HatMagician2Card { NextPlayMulti: > 1 })
            return this.Multi;
        return 1;
    }

    // 受到攻击后删除能力
    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (IsTriggerMulti(cardSource) && this.Owner == target)
        {
            // this.Flash();
            PowerCmd.Remove(this);
        }

        return base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }

    // 是否触发倍伤 攻击牌即触发 （后续优化 伤害大于0才触发？暂时没思路）
    public static bool IsTriggerMulti(CardModel? cardSource, decimal amount = 1) => cardSource is { Type: CardType.Attack } && amount > 0;

    // 外部获取已有的灼痕倍数
    public static decimal GetNowMulti(Creature target)
    {
        var multiPower = (MultiDamagePower?)target.Powers.FirstOrDefault(p => p is MultiDamagePower);
        var multi = multiPower?.Multi ?? 1;
        return multi;
    }
}