using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class PlasmaShield() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromPower<BrandYellowPower>()];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new BlockVar(7, ValueProp.Move), new Hat2Var(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplySelfPower<PlasmaShieldPower>(choiceContext, play, this.DynamicHat2Var.IntValue);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonBlock(play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(2);
        this.DynamicHat2Var.UpgradeValueBy(1);
    }
}