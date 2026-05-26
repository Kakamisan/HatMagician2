using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class SweetDreamPower : HatMagician2Power
{
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player != this.Owner.Player ? count : count + this.Amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != this.Owner.Player)
            return;
        List<CardModel> list = (await CardSelectCmd.FromHand(choiceContext, player, new CardSelectorPrefs(this.SelectionScreenPrompt, 1), null, this)).ToList();
        if (list.Count == 0)
            return;
        await CardPileCmd.Add(list, PileType.Draw, CardPilePosition.Top);
    }
}