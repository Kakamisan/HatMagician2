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
public class PhantasmalFlame() : HatMagician2Card(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApplyTarget => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(9, ValueProp.Move), new RepeatVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Ethereal];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplyTargetPower<BrandRedPower>(choiceContext, play, 1);
        await this.CommonApplyTargetPower<BrandBluePower>(choiceContext, play, 1);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonSingleAttack(choiceContext, play, this.DynamicVars.Repeat.IntValue);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        // this.DynamicBrandCost.UpgradeValueBy(-1);
        // this.DynamicVars.Repeat.UpgradeValueBy(1);
        this.DynamicVars.Damage.UpgradeValueBy(4);
    }
}