using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class InvertedFrost() : HatMagician2Card(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Blue;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => true;

    // public override bool HasFreeBrandApplyTarget => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromPower<BrandYellowPower>()];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(5, ValueProp.Move), new RepeatVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target!;
        if (target.HasPower<BrandBluePower>())
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, play, BrandColor.Yellow);
        }
        else
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, play, BrandColor.Blue);
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonSingleAttack(choiceContext, play, this.DynamicVars.Repeat.IntValue);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(2);
}