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
public class Crouch() : HatMagician2Card(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Blue;
    public override int BaseBrandColorCost => 1;

    public override bool HasBrandApplyTarget => true;

    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new BlockVar(7, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];
    
    // public override TargetType TargetType => this.HasEnoughEnergy() ? TargetType.AnyEnemy : TargetType.Self;
    public override TargetType? SubTargetType => TargetType.Self;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonBlock(play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(3);
}