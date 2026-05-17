using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class LightningStrike() : HatMagician2Card(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override int BaseBrandColorCost => 1;

    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    
    public override bool HasBrandApply => true;

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(7M, ValueProp.Move)];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        await this.OnPlayNormal(choiceContext, play);
    }
    
    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue) // 造成伤害，数值来源于卡牌的基础伤害属性
            .FromCard(this) // 伤害来源于这张卡牌
            .Targeting(play.Target!) // 伤害目标是玩家选择的目标
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3M);
}