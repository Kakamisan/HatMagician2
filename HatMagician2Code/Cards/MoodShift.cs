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
public class MoodShift() : HatMagician2Card(0, CardType.Skill, CardRarity.Common, TargetType.None)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new EnergyVar(1)];
    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, 2);
        var cards = await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner, prefs, null, this);
        await CardCmd.Discard(choiceContext, cards);
        await PlayerCmd.GainEnergy(this.DynamicVars.Energy.IntValue, this.Owner);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Energy.UpgradeValueBy(1);
}