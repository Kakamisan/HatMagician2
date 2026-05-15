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
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    // {
    //     if (side != this.Owner.Side)
    //         return;
    //     // VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_fire_burning");
    //     await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), this.Owner, this.Amount, ValueProp.Unpowered, null, null);
    // }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == this.Owner)
        {
            _ = CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), this.Owner, this.Amount, ValueProp.Unpowered | ValueProp.Unblockable, null, null);
        }
        return base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }
}