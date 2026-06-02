using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Enchantment;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Slumber() : HatMagician2Card(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApplyTarget => false;

    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [..HoverTipFactory.FromEnchantment<LightningEnchantment>(), HoverTipFactory.FromCard<Doze>(this.IsUpgraded)];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(2)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        for (int i = 0; i < this.DynamicVars.Cards.IntValue; i++)
        {
            var card = this.CombatState.CreateCard<Doze>(this.Owner);
            if (this.IsUpgraded)
                CardCmd.Upgrade(card);
            CardCmd.Enchant<LightningEnchantment>(card, 1);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, this.Owner);
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => base.OnUpgrade();
}