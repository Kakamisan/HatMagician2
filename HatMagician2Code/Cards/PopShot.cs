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
public class PopShot() : HatMagician2Card(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.None;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromCard<Insomnia>()];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(9, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonSingleAttack(choiceContext, play);
        if (this.CombatState == null) return;
        await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Insomnia>(this.Owner), PileType.Hand, this.Owner);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(4);
}