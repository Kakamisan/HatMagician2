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
public class MutualRadiance() : HatMagician2Card(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Orange;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(8, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;

        var others = this.CombatState.HittableEnemies.Where(e => e != play.Target).ToList();
        var enemy = this.Owner.RunState.Rng.CombatTargets.NextItem(others);

        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        if (enemy != null)
            await BrandPower.ApplyBrandPower(this, choiceContext, enemy, this.BaseBrandColor);

        await this.CommonSingleAttack(choiceContext, play);
        if (enemy != null)
            await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(enemy).WithHitFx("vfx/vfx_starry_impact").Execute(choiceContext);

        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;

        var others = this.CombatState.HittableEnemies.Where(e => e != play.Target).ToList();
        var enemy = this.Owner.RunState.Rng.CombatTargets.NextItem(others);

        await this.CommonSingleAttack(choiceContext, play);
        if (enemy != null)
            await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(enemy).WithHitFx("vfx/vfx_starry_impact").Execute(choiceContext);

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(4);
}