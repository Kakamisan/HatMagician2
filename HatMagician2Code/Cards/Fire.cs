using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Fire : HatMagician2Card
{
    public Fire() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        IsTest = true;
        BaseBrandColorCost = 1;
        BaseBrandColor = BrandColor.Red;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Retain);
}