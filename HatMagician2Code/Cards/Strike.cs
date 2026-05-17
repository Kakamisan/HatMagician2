using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Strike() : HatMagician2Card(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> Hat2CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(6, ValueProp.Move)];

    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep]; // 测试机制

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue) // 造成伤害，数值来源于卡牌的基础伤害属性
            // .WithHitCount(2)
            .FromCard(this) // 伤害来源于这张卡牌
            .Targeting(play.Target!) // 伤害目标是玩家选择的目标
            // .WithHitFx("vfx/vfx_starry_impact", "blunt_attack.mp3")
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3); // 升级后增加4点伤害
}