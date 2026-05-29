using HatMagician2.HatMagician2Code.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class TrancePower : HatMagician2Power
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Doze>()];

    public override async Task AfterSideTurnStartLate(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == this.Owner.Side && this.Owner.Player != null)
        {
            this.Flash();
            for (int i = 0; i < this.Amount; i++)
            {
                var card = this.Owner.CombatState?.CreateCard(ModelDb.Card<Doze>(), this.Owner.Player);
                if (card == null) return;
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, this.Owner.Player, CardPilePosition.Random));
            }
        }

        await base.AfterSideTurnStartLate(side, participants, combatState);
    }
}