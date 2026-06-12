using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.MonsterPowers;

public class PollutionPower : HatMagician2Power
{
    public const int BaseCardsLeft = 7;
    private const string CardsLeftKey = "CardsLeft";

    public override int DisplayAmount => this.DynamicVars[CardsLeftKey].IntValue;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new(CardsLeftKey, BaseCardsLeft)];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Target?.Player || cardPlay.Card.Type == CardType.Status)
            return;
        --this.DynamicVars[CardsLeftKey].BaseValue;
        this.InvokeDisplayAmountChanged();
        if (this.DynamicVars[CardsLeftKey].IntValue > 0)
            return;
        await Cmd.Wait(0.5f);

        var card = this.CombatState.CreateCard<ColorPollutionStatus>(cardPlay.Card.Owner);
        card.DynamicColor = cardPlay.Card.Owner.RunState.Rng.CombatEnergyCosts.NextItem([BrandColor.Red, BrandColor.Blue, BrandColor.Yellow]);
        card.TargetOwner = this.Owner;
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, null));

        this.Flash();
        this.DynamicVars[CardsLeftKey].BaseValue = BaseCardsLeft;
        this.InvokeDisplayAmountChanged();
    }
}