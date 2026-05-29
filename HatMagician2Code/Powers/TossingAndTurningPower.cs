using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class TossingAndTurningPower : HatMagician2Power
{
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != this.Owner)
            return;
        if (this.Owner.Player == null)
            return;
        this.Flash();
        for (int i = 0; i < this.Amount; i++)
        {
            Creature? enemy = this.Owner.Player.RunState.Rng.CombatTargets.NextItem(this.CombatState.HittableEnemies);
            if (enemy != null)
                await BrandPower.ApplyBrandPower(null, this.Owner, choiceContext, enemy, BrandColor.Yellow);
        }

        await base.AfterCardExhausted(choiceContext, card, causedByEthereal);
    }
}