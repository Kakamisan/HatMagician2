using HatMagician2.HatMagician2Code.Character;
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
    public override bool FakeDebuff => true;

    // 受到攻击减少1层
    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (props.IsPoweredAttack() && this.Owner == target && this.Owner.CombatState != null && amount > 0)
        {
            this.Flash();
            var others = this.Owner.CombatState.Enemies.Where(e => e.IsAlive && e != this.Owner);
            var modifyDamage = HatMagician2Mgr.ModifyChainDamage(this.Owner, amount, ValueProp.Unpowered, dealer, cardSource, this.Owner.CombatState);
            await CreatureCmd.Damage(choiceContext, others, new DamageVar(modifyDamage, ValueProp.Unpowered), this.Applier, null);
            await PowerCmd.Decrement(this);
        }

        await base.BeforeDamageReceived(choiceContext, target, amount, props, dealer, cardSource);
    }
}