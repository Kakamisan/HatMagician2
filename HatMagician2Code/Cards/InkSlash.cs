using BaseLib.Cards.Variables;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class InkSlash() : HatMagician2Card(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Color)];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new CalculationBaseVar(1), new ExtraDamageVar(3), new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => HatMagician2Mgr.GetBrandColorTypeCnt(card.Owner))
        //new CalculationExtraVar()
    ];
    // protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(this.DynamicVars.CalculatedDamage).FromCard(this).Targeting(play.Target!).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.ExtraDamage.UpgradeValueBy(1);
}