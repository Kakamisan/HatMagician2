using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

// 冰霜 - 本回合减少伤害 等效于负力量
public class FreezeStrengthPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Decimal ModifyDamageAdditive(Creature? target, Decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return this.Owner != dealer || !props.IsPoweredAttack() ? 0M : -this.Amount;
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != this.Owner.Side)
            return;
        this.Flash();
        await PowerCmd.Remove(this);
    }
}