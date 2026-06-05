using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Indifference() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromPower<GloomyPower>()];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new CustomCalculatedBlockVar("Indifference", ValueProp.Move).WithMultiplier((card, _) => ((Indifference)card).GetEnemiesGloomyCnt()),
        new("IndifferenceBase", 0), new("IndifferenceExtra", 1), new Hat2Var(3)
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonAoeApplyTargetPower<GloomyPower>(choiceContext, this.DynamicHat2Var.IntValue);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonBlock(play, ((CustomCalculatedBlockVar)this.GetDynamicVar("Indifference")).Calculate(play.Target));
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        // this.EnergyCost.UpgradeBy(-1);
        this.DynamicHat2Var.UpgradeValueBy(1);
        this.AddKeyword(CardKeyword.Retain);
    }

    private int GetEnemiesGloomyCnt()
    {
        if (this.CombatState == null) return 0;
        var block = 0;
        foreach (var e in this.CombatState.HittableEnemies)
        {
            if (e.Powers.FirstOrDefault(p => p is GloomyPower) is GloomyPower p2)
            {
                block += p2.Amount;
            }
        }

        return block;
    }
}