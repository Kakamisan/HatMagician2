using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandPurplePower : BrandPower
{
    public BrandPurplePower()
    {
        BaseBrandColor = BrandColor.Purple;
        BasePassiveVal = 2;
        BaseEvokeVal = 6;
        BaseFusionVal = 6;
    }

    // protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new PowerVar<StrengthPower>(4)); // 本回合失去力量

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        await base.OnEvoke(card);
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        await PowerCmd.Apply<TmpStrengthPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        await base.OnFusion(cardSource);
        if (this.Applier?.Player == null)
            return;
        await PaletteBottle.AddEnergy(this.Applier.Player, 1, this.BaseBrandColor);
        await PowerCmd.Apply<RoundDamagePower>(new ThrowingPlayerChoiceContext(), this.Owner, this.FusionVal, this.Applier, null);
    }
    
    public override Decimal ModifyDamageAdditive(Creature? target, Decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return this.Owner != dealer || !props.IsPoweredAttack() ? 0M : -this.PassiveVal;
    }
}