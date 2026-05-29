using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class AllNighter() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromCard<Insomnia>()];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new EnergyVar(2), new PowerVar<StrengthPower>(2), new PowerVar<DexterityPower>(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep, CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        await HatMagician2Mgr.AddEnergy(this.Owner, this.DynamicVars.Energy.IntValue, BrandColor.Purple);
        await this.CommonApplySelfPower<AllNighterStrPower>(choiceContext, play, this.DynamicVars.Strength.IntValue);
        await this.CommonApplySelfPower<AllNighterDexPower>(choiceContext, play, this.DynamicVars.Dexterity.IntValue);
        for (int i = 0; i < 2; i++)
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(this.Owner), PileType.Discard, this.Owner));
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        // this.DynamicVars.Energy.UpgradeValueBy(1);
        // this.DynamicVars.Strength.UpgradeValueBy(1);
        // this.DynamicVars.Dexterity.UpgradeValueBy(1);
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}