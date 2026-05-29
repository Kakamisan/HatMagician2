using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class CoffeePower : HatMagician2Power
{
    private bool _trigger;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (this._trigger) return;
        if (this.Owner != card.Owner.Creature) return;
        if (card.DeckVersion != null) return;
        if (this.Owner.Player == null) return;
        this._trigger = true;
        this.Flash();
        await PlayerCmd.GainEnergy(this.Amount, this.Owner.Player);
        await base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        this._trigger = false;
        await base.AfterSideTurnEndLate(choiceContext, side, participants);
    }
}