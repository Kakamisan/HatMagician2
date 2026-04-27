using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

public class Lightning : HatMagician2Card
{
    public Lightning() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        IsTest = true;
        BaseBrandColorCost = 1;
        BaseBrandColor = BrandColor.Yellow;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Retain);
}