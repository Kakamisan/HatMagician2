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
public class FinalCommand() : HatMagician2Card(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromCard<Doze>(true)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(13, ValueProp.Move), new Hat2Var(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        for (int i = 0; i < this.DynamicHat2Var.IntValue; i++)
        {
            var card = this.CombatState.CreateCard<Doze>(this.Owner);
            CardCmd.Upgrade(card);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, this.Owner, CardPilePosition.Random));
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        var card = this.CombatState.CreateCard<Doze>(this.Owner);
        CardCmd.Upgrade(card);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, this.Owner, CardPilePosition.Random));
        await this.CommonSingleAttack(choiceContext, play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(5);
}