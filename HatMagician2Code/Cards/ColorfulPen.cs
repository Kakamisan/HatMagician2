using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class ColorfulPen() : HatMagician2Card(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    private CardModel? _mockSelectedCard;

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Fire>(this.IsUpgraded), HoverTipFactory.FromCard<Lightning>(this.IsUpgraded), HoverTipFactory.FromCard<Ice>(this.IsUpgraded)
    ];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardModel? card;
        var player = this.Owner;
        if (player.Creature.CombatState is null)
            return;
        if (this._mockSelectedCard is null)
            card = await CardSelectCmd.FromChooseACardScreen(choiceContext, GenCards([ModelDb.Card<Fire>(), ModelDb.Card<Lightning>(), ModelDb.Card<Ice>()], player), this.Owner,
                true);
        else
            card = this._mockSelectedCard;

        if (card == null)
            return;
        //card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, this.Owner);
    }

    public IReadOnlyList<CardModel> GenCards(List<CardModel> cards, Player player)
    {
        if (player.Creature.CombatState is null)
            return [];
        List<CardModel> list1 = [ModelDb.Card<Fire>(), ModelDb.Card<Lightning>(), ModelDb.Card<Ice>()];
        List<CardModel> forCombat = [];
        forCombat.AddRange(list1.Select(canonicalCard => player.Creature.CombatState.CreateCard(canonicalCard, player)));

        if (this.IsUpgraded)
        {
            foreach (var card in forCombat)
            {
                CardCmd.Upgrade(card);
            }
        }

        return forCombat;
    }

    // protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);

    public void MockSelectedCard(CardModel card)
    {
        this.AssertMutable();
        this._mockSelectedCard = card;
    }
}