using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class MindWandering() : HatMagician2Card(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromCard<Doze>(this.IsUpgraded)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        foreach (var original in PileType.Hand.GetPile(this.Owner).Cards.Where(c => c.IsTransformable && !c.Keywords.Contains(HatMagician2Keywords.Sleep)).ToList())
        {
            var card = this.CombatState.CreateCard<Doze>(this.Owner);
            if (this.IsUpgraded) CardCmd.Upgrade(card);
            await CardCmd.Transform(original, card);
        }

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => base.OnUpgrade();
}