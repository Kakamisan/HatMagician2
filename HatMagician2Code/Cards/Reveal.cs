using BaseLib.Cards.Variables;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Reveal() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromPower<NightmarePower>()];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new Hat2Var(1), new("CalcBase", 0), new("CalcExtra", 1),
        new CustomCalculatedVar("Calc").WithMultiplier((card, target) => ((Reveal)card).GetCalcAmount())
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Dream, CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var amount = this.Owner.PlayerCombatState?.AllCards.Count(c => c.Keywords.Contains(HatMagician2Keywords.Dream)) ?? 0;
        if (amount > 0)
            await this.CommonAoeApplyTargetPower<NightmarePower>(choiceContext, amount);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);

    private int GetCalcAmount() => this.Owner.PlayerCombatState?.AllCards.Count(c => c.Keywords.Contains(HatMagician2Keywords.Dream)) ?? 0;
}