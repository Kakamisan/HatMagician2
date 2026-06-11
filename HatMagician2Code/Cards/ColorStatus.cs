using BaseLib.Cards.Variables;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Monsters;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class ColorStatus() : HatMagician2Card(0, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override BrandColor BaseBrandColor => this.DynamicColor;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApply => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new("BranchBase", 0), new("BranchExtra", 1), new CustomCalculatedVar("Branch").WithMultiplier((card, _) => ((ColorStatus)card).GetColor())
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    public BrandColor DynamicColor = BrandColor.Red;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = this.CombatState?.Enemies.FirstOrDefault(e => e.Monster is ColorFinderPainting);
        if (target == null) return;
        await BrandPower.ApplyBrandPower(this, choiceContext, target, this.DynamicColor);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => base.OnUpgrade();

    public override int MaxUpgradeLevel => 0;

    private int GetColor()
    {
        return (int)this.DynamicColor;
    }
}