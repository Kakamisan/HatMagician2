using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public sealed class NightmarePower : HatMagician2Power
{
    public override PowerType Type => PowerType.Debuff;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("CalcAmount", 0)];

    private bool _stun;

    private const int MaxAmount = 4; // 最大层数

    private const decimal AdditiveRate = 12.5M; // 加成值 12.5%

    public override int DisplayAmount => this.Amount > MaxAmount ? MaxAmount : this.Amount;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == this.Owner && props.IsPoweredAttack() && dealer != null && cardSource != null)
        {
            return 1M + this.DisplayAmount * AdditiveRate / 100;
        }

        return base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource, cardPlay);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            this.GetDynamicVar("CalcAmount").BaseValue = this.DisplayAmount * AdditiveRate;
            if (!this._stun && this.Amount >= MaxAmount)
            {
                this._stun = true;
                // 击晕
                this.Flash();
                VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_attack_lightning");
                SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_passive");
                await CreatureCmd.Stun(this.Owner);
            }
        }

        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}