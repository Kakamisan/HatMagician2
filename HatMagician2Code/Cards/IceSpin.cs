using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class IceSpin() : HatMagician2Card(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.Blue;
    public override int BaseBrandColorCost => 2;

    public override bool HasBrandApply => true;

    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(6, ValueProp.Move)];

    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];
    public override bool IsAoeAttack => true;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        foreach (Creature enemy in this.CombatState.HittableEnemies)
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, enemy, this.BaseBrandColor);
        }
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(this.CombatState).WithHitFx("vfx/vfx_starry_impact").Execute(choiceContext);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2);
        this.DynamicBrandCost.UpgradeValueBy(-1);
    }
}