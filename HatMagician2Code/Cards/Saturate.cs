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
public class Saturate() : HatMagician2Card(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(6, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        var power = play.Target!.Powers.FirstOrDefault(p => p is BrandPower) as BrandPower;
        var color = power?.BaseBrandColor ?? BrandColor.None;
        var enemys = this.CombatState.GetTeammatesOf(play.Target).Where(e => e.IsAlive && e != play.Target).ToList();
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue) // 造成伤害，数值来源于卡牌的基础伤害属性
            .FromCard(this) // 伤害来源于这张卡牌
            .Targeting(play.Target!) // 伤害目标是玩家选择的目标
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
        foreach (var enemy in enemys)
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, enemy, color);
        }

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3);
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}