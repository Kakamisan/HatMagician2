using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Rolling() : HatMagician2Card(0, CardType.Skill, CardRarity.Common, TargetType.None)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    // protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep, CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, 1);
        CardModel? card1 = (await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner, prefs, null, this)).FirstOrDefault();
        if (card1 != null)
        {
            await CardCmd.Discard(choiceContext, card1);
        }

        CardSelectorPrefs prefs2 = new CardSelectorPrefs(this.SelectionScreenPrompt2, 1);
        CardModel? card2 = (await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Discard.GetPile(this.Owner).Cards.Where(c => c is not Rolling).ToList(), this.Owner, prefs2))
            .FirstOrDefault();
        if (card2 == null) return;
        await CardPileCmd.Add(card2, PileType.Hand);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.RemoveKeyword(CardKeyword.Exhaust);
}