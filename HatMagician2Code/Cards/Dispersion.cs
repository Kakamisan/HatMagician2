using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Dispersion() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandColorCostX => true;
    // public override bool HasBrandApply => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new Hat2Var(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 获得2X绘色
        var add = this.ResolveBrandColorCostXValue() * this.DynamicHat2Var.IntValue;
        if (add > 0)
        {
            await HatMagician2Mgr.AddEnergy(this.Owner, add, BrandColor.Yellow);
            await HatMagician2Mgr.AddEnergy(this.Owner, add, BrandColor.Blue);
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}