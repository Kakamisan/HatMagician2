using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Enchantment;

public class LightningEnchantment : Hat2Enchantment
{
    public override bool ShowAmount => false;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (this.Card is HatMagician2Card) return;
        if (cardPlay == null) return;
        await BrandPower.ApplyBrandPower(this.Card, choiceContext, cardPlay, BrandColor.Yellow);
        await base.OnPlay(choiceContext, cardPlay);
    }

    public override bool CanEnchant(CardModel card)
    {
        return card.TargetType == TargetType.AnyEnemy;
    }

    protected override void OnEnchant()
    {
        if (this.Card is HatMagician2Card card2)
        {
            card2.PreEnchantmentFunc = async (context, play) => { await BrandPower.ApplyBrandPower(card2, context, play, BrandColor.Yellow); };
        }

        base.OnEnchant();
    }
}