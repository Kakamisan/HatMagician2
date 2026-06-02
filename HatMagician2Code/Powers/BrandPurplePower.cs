using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandPurplePower : BrandPower
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
        await PowerCmd.Apply<GloomyPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
        if (this.Applier == null) return;
        await PowerCmd.Apply<CollectDarkPower>(new ThrowingPlayerChoiceContext(), this.Applier, this.EvokeVal2, this.Applier, null);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        if (this.IsOnFusionEd) return;
        if (this.Applier?.Player == null) return;
        await base.OnFusion(cardSource);
        VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_starry_impact");
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, (int)this.FusionVal2, this.BaseBrandColor);
        // await PowerCmd.Apply<GloomyPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.FusionVal, this.Applier, null);
        await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.FusionVal, this.Applier, null);
    }

    // public override Decimal ModifyDamageAdditive(Creature? target, Decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    // {
    //     return this.Owner != dealer || !props.IsPoweredAttack() ? 0M : -this.PassiveVal;
    // }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == this.Owner)
        {
            await GloomyPower.DealGloomyDamage(this.Owner, this.PassiveVal, this.Applier);
        }

        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }

    public static async Task UsePassive(BrandPower power, CardModel? card = null, int cnt = 1)
    {
        for (int i = 0; i < cnt; i++)
        {
            VfxCmd.PlayOnCreature(power.Owner, "vfx/vfx_starry_impact");
            // await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), power.Owner, power.PassiveVal, card != null ? card.Owner.Creature : power.Applier, card);
            await PowerCmd.Apply<GloomyPower>(new ThrowingPlayerChoiceContext(), power.Owner, power.PassiveVal, card != null ? card.Owner.Creature : power.Applier, card);
        }

        await Task.CompletedTask;
    }
}