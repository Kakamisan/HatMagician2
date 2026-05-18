using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class SleepTalk() : HatMagician2Card(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Sleep)];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(8, ValueProp.Move), new Hat2Var(4)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Dream];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    public decimal IncreaseDamage;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(this.CombatState).WithHitFx("vfx/vfx_starry_impact").Execute(choiceContext);
        // await PowerCmd.Apply<WeakPower>(choiceContext, this.CombatState.HittableEnemies, this.DynamicVars.Weak.BaseValue, this.Owner.Creature, this);
        await base.OnPlayNormal(choiceContext, play);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return cardSource != this ? 0 : this.IncreaseDamage;
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2);
        this.DynamicHat2Var.UpgradeValueBy(2);
    }

    // 每打出一张睡衣卡 伤害增加N
    public override Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == this.Owner && cardPlay.Card.Keywords.Contains(HatMagician2Keywords.Sleep))
        {
            this.IncreaseDamage += this.DynamicHat2Var.IntValue;
        }

        return base.AfterCardPlayedLate(choiceContext, cardPlay);
    }

    // 回合结束重置伤害加成
    public override Task AfterTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side)
    {
        this.IncreaseDamage = 0;
        return base.AfterTurnEndLate(choiceContext, side);
    }
}