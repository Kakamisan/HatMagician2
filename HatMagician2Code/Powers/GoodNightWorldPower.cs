using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Powers;

public class GoodNightWorldPower : HatMagician2Power
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == this.Owner.Side)
        {
            var enemies = this.CombatState.HittableEnemies.ToList();
            for (int i = 0; i < this.Amount; i++)
            {
                foreach (var e in enemies)
                {
                    await BrandPower.ApplyBrandPower(null, this.Owner, new ThrowingPlayerChoiceContext(), e, BrandColor.Blue);
                }
            }
        }

        await base.AfterSideTurnEnd(choiceContext, side, participants);
    }
}