using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class GoodNightWorldPower : HatMagician2Power
{
    private static BrandColor CostColor => BrandColor.Blue;

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == this.Owner.Side && this.Owner.Player != null)
        {
            BrandColorEnergyState? state = HatMagician2Mgr.Instance?.GetState(this.Owner.Player);
            if (state != null && state.GetEnergy(CostColor) > 0)
            {
                await state.SpendEnergy(CostColor, 1);
                await CreatureCmd.GainBlock(this.Owner, this.Amount, ValueProp.Unpowered, null);
            }
        }

        await base.BeforeSideTurnEndEarly(choiceContext, side, participants);
    }
}