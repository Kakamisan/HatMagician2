using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Submergence() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromPower<GloomyPower>()];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new CustomCalculatedVar("Submergence").WithMultiplier((card, target) => ((Submergence)card).GetGloomyCnt(target)),
        new("SubmergenceBase", 0), new("SubmergenceExtra", 1), new Hat2Var(1),
        new CustomCalculatedVar("SubmergenceShow").WithMultiplier((card, target) => ((Submergence)card).GetGloomyCntShow(target)),
        new("SubmergenceShowBase", 0), new("SubmergenceShowExtra", 1)
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Dream];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplyTargetPower<GloomyPower>(choiceContext, play,
            ((CustomCalculatedVar)this.GetDynamicVar("Submergence")).Calculate(play.Target!) * (1 + this.DynamicHat2Var.IntValue));
        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplyTargetPower<GloomyPower>(choiceContext, play, ((CustomCalculatedVar)this.GetDynamicVar("Submergence")).Calculate(play.Target!));
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicHat2Var.UpgradeValueBy(1);

    private int GetGloomyCnt(Creature? target)
    {
        if (target?.GetPower<GloomyPower>() is { } power)
        {
            return (int)Math.Floor((double)power.Amount / 3);
        }

        return 0;
    }

    private int GetGloomyCntShow(Creature? target)
    {
        var multi = this.HasEnoughEnergy() ? this.DynamicHat2Var.IntValue : 0;
        if (target?.GetPower<GloomyPower>() is { } power)
        {
            return (int)Math.Floor((double)power.Amount / 3 * (1 + multi));
        }

        return 0;
    }
}