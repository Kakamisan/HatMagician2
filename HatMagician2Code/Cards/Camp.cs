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
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Camp() : HatMagician2Card(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new BlockVar(14, ValueProp.Move), new Hat2Var(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonBlock(play);
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, this.DynamicHat2Var.IntValue);
        CardModel[] array = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, null, this)).ToArray();
        if (array.Length == 0) return;
        await CardPileCmd.Add(array, PileType.Draw, CardPilePosition.Top);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(5);
}