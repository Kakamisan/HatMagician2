using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class DarkMoon() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(1), new Hat2Var(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, this.DynamicHat2Var.IntValue + this.DynamicVars.Cards.IntValue, this.Owner);
        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.IntValue, this.Owner);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1);
}