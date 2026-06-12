using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandPurplePower : BrandPower, IHatMagician2AbstractModel
{
    public override BrandColor BaseBrandColor => BrandColor.Purple;
    protected override decimal BasePassiveVal => 4;
    protected override decimal BaseEvokeVal => 6;
    protected override decimal BaseEvokeVal2 => 2;
    protected override decimal BaseFusionVal => 6;
    protected override decimal BaseFusionVal2 => 1;

    // protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new PowerVar<StrengthPower>(4)); // 本回合失去力量

    // protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FreezeStrengthPower>()];

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (!this.Owner.IsAlive) return;
        if (this.Owner.CombatState == null) return;
        await base.OnEvoke(card);
        VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_starry_impact");
        // await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
        await PowerCmd.Apply<GloomyPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, card?.Owner.Creature ?? this.Applier, null);
        if (this.Applier == null) return;
        if (this.Owner.Side == CombatSide.Enemy)
            await PowerCmd.Apply<CollectDarkPower>(new ThrowingPlayerChoiceContext(), card?.Owner.Creature ?? this.Applier, this.EvokeVal2, card?.Owner.Creature ?? this.Applier, null);
    }

    protected override async Task OnFusion(CardModel? cardSource, Creature? oldApplier = null)
    {
        if (this.IsOnFusionEd) return;
        await base.OnFusion(cardSource, oldApplier);
        VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_starry_impact");
        await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.FusionVal, cardSource?.Owner.Creature ?? this.Applier, null);
    }

    public async Task AfterSingleDamageReceived(PlayerChoiceContext choiceContext, ICombatState combatState, List<Creature> targets, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == this.Owner && (dealer.Side == CombatSide.Enemy && targets[0].Side == CombatSide.Player || props.IsPoweredAttack() && cardSource != null))
        {
            await GloomyPower.DealGloomyDamage(this.Owner, this.PassiveVal, this.Owner);
        }

        await Task.CompletedTask;
    }

    public static async Task UsePassive(BrandPower power, CardModel? card = null, int cnt = 1)
    {
        for (int i = 0; i < cnt; i++)
        {
            VfxCmd.PlayOnCreature(power.Owner, "vfx/vfx_starry_impact");
            // await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), power.Owner, power.PassiveVal, card != null ? card.Owner.Creature : power.Applier, card);
            await PowerCmd.Apply<GloomyPower>(new ThrowingPlayerChoiceContext(), power.Owner, power.PassiveVal, card?.Owner.Creature ?? power.Applier, card);
        }

        await Task.CompletedTask;
    }
}