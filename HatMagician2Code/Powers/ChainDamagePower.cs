using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

// 连锁 伤害 - 下次受到攻击同时对所有其他敌人造成等量伤害 黄昏印记附属能力
// 玩家侧 - 下次攻击对友军也生效
public class ChainDamagePower : HatMagician2Power
{
    public override PowerType Type => PowerType.Debuff;
    public override bool FakeDebuff => true;

    private bool _ready2Decrement;

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (props.IsPoweredAttack() && this.Owner == target && target.Side == CombatSide.Enemy && this.Owner.CombatState != null && amount > 0)
        {
            this.Flash();
            var others = this.Owner.CombatState.Enemies.Where(e => e.IsAlive && e != this.Owner);
            var modifyDamage = HatMagician2Mgr.ModifyChainDamage(this.Owner, amount, ValueProp.Unpowered, dealer, cardSource, this.Owner.CombatState);
            await CreatureCmd.Damage(choiceContext, others, new DamageVar(modifyDamage, ValueProp.Unpowered), HatMagician2Mgr.GetDamageApplierUtil(cardSource, this.Applier), null);
            this._ready2Decrement = true;
        }

        if (props.IsPoweredAttack() && dealer == this.Owner && this.Owner is { Side: CombatSide.Player, CombatState: not null } && amount > 0)
        {
            this.Flash();
            var allies = this.Owner.CombatState.Allies.ToList();
            await CreatureCmd.Damage(choiceContext, allies, new DamageVar(amount, ValueProp.Unpowered), HatMagician2Mgr.GetDamageApplierUtil(cardSource, this.Applier), null);
            this._ready2Decrement = true;
        }

        await base.BeforeDamageReceived(choiceContext, target, amount, props, dealer, cardSource);
    }

    // 触发后减少一层
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this._ready2Decrement)
        {
            this._ready2Decrement = false;
            await PowerCmd.Decrement(this);
        }

        await base.AfterCardPlayedLate(choiceContext, cardPlay);
    }
}