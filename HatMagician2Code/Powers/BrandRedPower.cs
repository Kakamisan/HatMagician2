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

public class BrandRedPower : BrandPower, IHatMagician2AbstractModel
{
    public override BrandColor BaseBrandColor => BrandColor.Red;
    protected override decimal BasePassiveVal => 6;
    protected override decimal BaseEvokeVal => 1;

    protected override string ChannelSfx => "event:/sfx/characters/defect/defect_dark_channel";

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        await base.OnEvoke(card);
        VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_fire_burning");
        // 如果是攻击牌则直接触发N倍伤害效果 否则添加灼痕
        // Aoe不要设置倍率 除非只剩一只怪了
        if (MultiDamagePower.IsTriggerMulti(card) && (!card!.IsAoeAttack || card.CombatState!.GetTeammatesOf(this.Owner).Count == 1))
        {
            card.SetNextPlayMultiAdd(this.EvokeVal - 1);
        }
        else
        {
            var stack = this.Owner.HasPower<MultiDamagePower>() ? this.EvokeVal - 1 : this.EvokeVal;
            await PowerCmd.Apply<MultiDamagePower>(new ThrowingPlayerChoiceContext(), this.Owner, stack, card?.Owner.Creature ?? this.Applier, null);
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Enemy) return;
        await this.OnPassive();
        await base.AfterSideTurnStart(side, participants, combatState);
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
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), power.Owner, power.PassiveVal, ValueProp.Unpowered, card?.Owner.Creature ?? power.Applier, card);
        }

        await Task.CompletedTask;
    }

    // 仅预览时生效倍数
    public int TryModifyMultiDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel card)
    {
        return target == this.Owner && props.IsPoweredAttack() && card is HatMagician2Card card2 && card2.IsEvokeCard() && !card2.IsBrandAppliedBeforeAttack
            ? (int)this.EvokeVal - 1
            : 0;
    }

    // 给自己的激活数值+1 默认2倍伤害 其他能力的加成则是对激活值=1的基础上加成
    public bool TryModifyEvokeValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power == this)
        {
            modifiedVal = originVal + 1;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }
}