using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

// 阴郁 - 攻击时失去1点生命
public class GloomyPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Debuff;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == this.Owner && target == this.CombatState.PlayerCreatures.FirstOrDefault(c => c.IsAlive))
        {
            await DealGloomyDamage(this.Owner, this.Amount, this.Owner);
        }

        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }

    // 造成来源于阴郁的伤害
    public static async Task DealGloomyDamage(Creature target, decimal damage, Creature? applier = null)
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, damage, ValueProp.Unpowered | ValueProp.Unblockable, applier, null);
        await HatMagician2Mgr.AfterGloomyDamage(target, damage, applier);
    }
}