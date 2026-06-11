using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Powers;

public class AfterglowPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Debuff;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        const BrandColor color = BrandColor.Orange;
        if (side == CombatSide.Player && this.Applier?.Player != null && HatMagician2Mgr.HasEnoughEnergy(this.Applier.Player, color, 1))
        {
            this.Flash();
            await HatMagician2Mgr.SpendEnergy(this.Applier.Player, 1, color);
            await BrandPower.ApplyBrandPower(null, this.Applier, choiceContext, this.Owner, color);
            await PowerCmd.Decrement(this);
        }

        await base.AfterSideTurnEnd(choiceContext, side, participants);
    }
}