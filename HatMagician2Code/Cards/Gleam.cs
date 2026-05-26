using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Gleam() : HatMagician2Card(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new RepeatVar(1), new DamageVar(7, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonAoeAttack(choiceContext, play, this.DynamicVars.Repeat.IntValue);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonAoeAttack(choiceContext, play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Repeat.UpgradeValueBy(1);
}