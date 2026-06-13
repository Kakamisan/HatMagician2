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
public class TossingAndTurning() : HatMagician2Card(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonApplySelfPower<TossingAndTurningPower>(choiceContext, play, 1);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}