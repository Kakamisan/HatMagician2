using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

// 阴郁 - 每回合受到伤害
public class RoundDamagePower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != this.Owner.Side)
            return;
        // VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_fire_burning");
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), this.Owner, this.Amount, ValueProp.Unpowered, null, null);
    }
}