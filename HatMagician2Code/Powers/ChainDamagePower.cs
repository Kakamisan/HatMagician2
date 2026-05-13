using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

// 连锁 伤害 - 下次受到攻击同时对所有其他敌人造成等量伤害 黄昏印记附属能力
public class ChainDamagePower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 受到攻击减少1层
    public override Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (props.IsPoweredAttack() && this.Owner == target && this.Owner.CombatState != null && amount > 0)
        {
            this.Flash();
            var others = this.Owner.CombatState.Enemies.Where(e => e.IsAlive && e != this.Owner);
            CreatureCmd.Damage(choiceContext, others, new DamageVar(amount, ValueProp.Unpowered), this.Applier, null);
            PowerCmd.Decrement(this);
        }

        return base.BeforeDamageReceived(choiceContext, target, amount, props, dealer, cardSource);
    }
}