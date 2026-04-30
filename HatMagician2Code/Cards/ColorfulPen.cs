using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class ColorfulPen : HatMagician2Card
{
    private CardModel? _mockSelectedCard;

    public ColorfulPen() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        IsTest = true;
        // BaseBrandColorCost = 1;
        // BaseBrandColor = BrandColor.Blue;
    }

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Fire>(), HoverTipFactory.FromCard<Lightning>(), HoverTipFactory.FromCard<Ice>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
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
        card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, this.Owner);
    }

    public static IReadOnlyList<CardModel> GenCards(List<CardModel> cards, Player player)
    {
        if (player.Creature.CombatState is null)
            return [];
        List<CardModel> list1 = [ModelDb.Card<Fire>(), ModelDb.Card<Lightning>(), ModelDb.Card<Ice>()];
        List<CardModel> forCombat = [];
        forCombat.AddRange(list1.Select(canonicalCard => player.Creature.CombatState.CreateCard(canonicalCard, player)));

        return forCombat;
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);

    public void MockSelectedCard(CardModel card)
    {
        this.AssertMutable();
        this._mockSelectedCard = card;
    }
}