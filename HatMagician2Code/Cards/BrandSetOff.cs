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
using MegaCrit.Sts2.GameInfo.Objects;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class BrandSetOff : HatMagician2Card
{
    public BrandSetOff() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        IsTest = true;
        // BaseBrandColorCost = 1;
        // BaseBrandColor = BrandColor.Blue;
    }

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(8, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [HatMagician2Keywords.Evoke];

    // protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandEvoke(this, choiceContext, play);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue) // 造成伤害，数值来源于卡牌的基础伤害属性
            // .WithHitCount(2)
            .FromCard(this) // 伤害来源于这张卡牌
            .Targeting(play.Target!) // 伤害目标是玩家选择的目标
            // .WithHitFx("vfx/vfx_starry_impact", "blunt_attack.mp3")
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3);
}