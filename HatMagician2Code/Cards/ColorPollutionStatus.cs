using BaseLib.Cards.Variables;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(StatusCardPool))]
public class ColorPollutionStatus() : HatMagician2Card(0, CardType.Status, CardRarity.Status, TargetType.Self)
{
    public override BrandColor BaseBrandColor => this.DynamicColor;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApply => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new("BranchBase", 0), new("BranchExtra", 1), new CustomCalculatedVar("Branch").WithMultiplier((card, _) => ((ColorPollutionStatus)card).GetColor()),
        new DamageVar(15, ValueProp.Unpowered | ValueProp.Move)
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    public BrandColor DynamicColor = BrandColor.Red;
    public Creature? TargetOwner;

    protected override bool IsPlayableSub => this.HasEnoughEnergy();
    public override bool HasTurnEndInHandEffect => true;
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(null, this.TargetOwner ?? this.Owner.Creature, choiceContext, this.Owner.Creature, this.DynamicColor);
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

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.Damage(choiceContext, this.Owner.Creature, this.DynamicVars.Damage, this);
    }
}