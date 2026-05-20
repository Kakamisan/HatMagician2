using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandRedPower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.Red;
    protected override decimal BasePassiveVal => 6;
    protected override decimal BaseEvokeVal => 2;
    protected override decimal BaseFusionVal => 0;

    protected override string ChannelSfx => "event:/sfx/characters/defect/defect_dark_channel";

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        await base.OnEvoke(card);
        VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_fire_burning");
        // 如果是攻击牌则直接触发N倍伤害效果 否则添加灼痕
        if (WillTriggerMultiDamage(card))
        {
            card!.SetNextPlayMulti(WillTriggerMulti(this));
        }
        else
        {
            var stack = this.Owner.HasPower<MultiDamagePower>() ? this.EvokeVal - 1 : this.EvokeVal;
            await PowerCmd.Apply<MultiDamagePower>(new ThrowingPlayerChoiceContext(), this.Owner, stack, card?.Owner.Creature, null);
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != this.Owner.Side) return;
        await this.OnPassive();
        await base.AfterSideTurnStart(side, combatState);
    }

    protected override async Task OnPassive(bool setFlag = true)
    {
        if (!this.Owner.IsAlive) return;
        await base.OnPassive(setFlag);
        await UsePassive(this);
    }

    public static async Task UsePassive(BrandPower power, CardModel? card = null, int cnt = 1)
    {
        for (int i = 0; i < cnt; i++)
        {
            VfxCmd.PlayOnCreature(power.Owner, "vfx/vfx_fire_burning");
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), power.Owner, power.PassiveVal, ValueProp.Unpowered, card != null ? card.Owner.Creature : power.Applier, card);
        }

        await Task.CompletedTask;
    }

    // 只用于预览计算伤害 实际倍率在card.NextPlayMulti
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return WillTriggerMultiDamage(cardSource) && target == this.Owner
            ? WillTriggerMulti(this)
            : base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
    }

    // 是否即将刻印火焰印记并造成伤害
    // 只用于预览计算伤害 实际倍率在card.NextPlayMulti 若实际倍率已设置 则不触发本模块的加伤
    public static bool WillTriggerMultiDamage(CardModel? cardSource, Creature? target)
    {
        return WillTriggerMultiDamage(cardSource) && target?.HasPower<BrandRedPower>() == true;
    }

    private static bool WillTriggerMultiDamage(CardModel? cardSource)
    {
        return cardSource is HatMagician2Card card && card.IsEvokeCard() && card is { Type: CardType.Attack, NextPlayMulti: 1, IsBrandApplied: false, IsAoeAttack: false };
    }

    // 即将触发的攻击倍数
    private static decimal WillTriggerMulti(BrandRedPower power)
    {
        // 灼痕已有倍数+本次刻印的增加1倍
        return MultiDamagePower.GetNowMulti(power.Owner) + 1;
    }
}