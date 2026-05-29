using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class DreamFall() : HatMagician2Card(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Dream];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var cards = PileType.Draw.GetPile(this.Owner).Cards
            .Where(c => !c.Keywords.Contains(HatMagician2Keywords.Dream) && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList();
        if (cards.Count <= 0) return;
        // var selection = this.IsUpgraded switch
        // {
        //     true => this.SelectionScreenPrompt2,
        //     _ => this.SelectionScreenPrompt
        // };
        var cards2 = await CardSelectCmd.FromSimpleGrid(choiceContext, cards, this.Owner, new CardSelectorPrefs(this.SelectionScreenPrompt, 0, this.DynamicVars.Cards.IntValue));
        foreach (var card in cards2)
        {
            card.AddKeyword(HatMagician2Keywords.Dream);
        }

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1);
}