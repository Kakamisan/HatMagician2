using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

// 阴郁 - 攻击时失去1点生命
public class GloomyPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public override PowerType Type => PowerType.Debuff;

    public async Task AfterSingleDamageReceived(PlayerChoiceContext choiceContext, ICombatState combatState, List<Creature> targets, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == this.Owner && (dealer.Side == CombatSide.Enemy && targets[0].Side == CombatSide.Player || props.IsPoweredAttack() && cardSource != null))
        {
            await DealGloomyDamage(this.Owner, this.Amount, this.Owner);
        }

        await Task.CompletedTask;
    }

    // 造成来源于阴郁的伤害
    public static async Task DealGloomyDamage(Creature target, decimal damage, Creature? applier = null)
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, damage, ValueProp.Unpowered | ValueProp.Unblockable, applier, null);
        await HatMagician2Mgr.AfterGloomyDamage(target, damage, applier);
    }
}