using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(TokenCardPool))]
public class Lightning() : HatMagician2Card(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
{
    public override int BaseBrandColorCost => 2;

    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    
    public override bool HasBrandApplyTarget => true;

    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    // protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Retain);
    protected override void OnUpgrade() => this.DynamicBrandCost.UpgradeValueBy(-1);
}