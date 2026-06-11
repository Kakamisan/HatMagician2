using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class BlearyEyed() : HatMagician2Card(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    public override int BaseBrandColorCost => 1;

    public override bool HasBrandApplyTarget => true;
    public override TargetType? SubTargetType => TargetType.None;

    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromCard<Doze>(this.IsUpgraded)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new CardsVar(1), new DamageVar(6, ValueProp.Unpowered | ValueProp.Unblockable | ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var card = this.Owner.Creature.CombatState?.CreateCard(ModelDb.Card<Doze>(), this.Owner);
        if (card == null) return;
        if (this.IsUpgraded) CardCmd.Upgrade(card);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, this.Owner, CardPilePosition.Top));
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => base.OnUpgrade();
}