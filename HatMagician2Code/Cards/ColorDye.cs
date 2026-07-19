using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(TokenCardPool))]
public class ColorDye() : HatMagician2Card(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => this.DynamicColor;
    public override int BaseBrandColorCost => -1;
    public override bool HasFreeBrandApplyTarget => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new("Branch", 0), new CardsVar(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    public BrandColor DynamicColor = BrandColor.None;
    private const int DynamicCost = 0;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.DynamicColor);
        // await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.IntValue, this.Owner);
        await base.OnPlayNormal(choiceContext, play);
    }

    // public override int MaxUpgradeLevel => 0;

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1);

    public void SetColor(BrandColor color)
    {
        this.DynamicColor = color;
        this.GetDynamicVar("Branch").BaseValue = (int)color;
        this.DynamicBrandCost.BaseValue = DynamicCost;
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card == this)
        {
            await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.IntValue, this.Owner);
        }

        await base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }
}