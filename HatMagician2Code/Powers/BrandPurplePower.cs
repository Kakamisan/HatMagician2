using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandPurplePower : BrandPower
{
    public BrandPurplePower()
    {
        BaseBrandColor = BrandColor.Purple;
        BasePassiveVal = 25; // 削减伤害百分比 
        BaseEvokeVal = 6; // 消亡层数
        BaseFusionVal = 1;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new PowerVar<StrengthPower>(4)); // 本回合失去力量

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        // VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_attack_lightning");
        await base.OnEvoke(card);
        await PowerCmd.Apply<DyingStarPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.DynamicVars.Strength.BaseValue, this.Applier, null);
        await PowerCmd.Apply<DemisePower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        await base.OnFusion(cardSource);
        if (this.Applier?.Player == null)
            return;
        await PaletteBottle.AddEnergy(this.Applier.Player, (int)this.FusionVal, this.BaseBrandColor);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return this.Owner != dealer || !props.IsPoweredAttack() ? 1M : (100 - this.PassiveVal) / 100;
    }
}