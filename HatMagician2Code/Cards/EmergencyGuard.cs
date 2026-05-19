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
public class EmergencyGuard() : HatMagician2Card(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.Blue;
    public override int BaseBrandColorCost => 1;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new BlockVar(7, ValueProp.Move), new Hat2Var(5)];
    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicHat2Var.BaseValue, ValueProp.Move, play);
        await this.OnPlayNormal(choiceContext, play);
        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, null, this)).FirstOrDefault();
        if (card == null) return;
        await CardCmd.Exhaust(choiceContext, card);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(2);
        this.DynamicHat2Var.UpgradeValueBy(2);
        //this.DynamicBrandCost.UpgradeValueBy(-1);
    }
}