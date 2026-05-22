using BaseLib.Extensions;
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
public class Sketch() : HatMagician2Card(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApply => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Color)];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move), new Hat2Var(3),
        new CalculationBaseVar(0), new CalculationExtraVar(1), new CalculatedVar("CalculatedCards").WithMultiplier((card, _) => ((Sketch)card).CalcDrawNum())
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonSingleAttack(choiceContext, play);
        var draw = this.GetDynamicVar("CalculatedCards").PreviewValue;
        if (draw > 0)
        {
            await CardPileCmd.Draw(choiceContext, draw, this.Owner);
        }

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3);
        this.DynamicHat2Var.UpgradeValueBy(-1);
    }

    private int CalcDrawNum()
    {
        int cnt = HatMagician2Mgr.GetBrandColorTypeCnt(this.Owner);
        int draw = (int)Math.Floor(cnt / this.DynamicHat2Var.BaseValue);
        return draw;
    }
}