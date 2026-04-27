using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandBluePower : BrandPower
{
    public BrandBluePower()
    {
        BaseBrandColor = BrandColor.Blue;
        BasePassiveVal = 1;
        BaseEvokeVal = 3;
    }

    protected override string ChannelSfx => "event:/sfx/characters/defect/defect_frost_channel";

    // protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        await base.OnEvoke(card);
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_starry_impact");
        await PowerCmd.Apply<TmpStrengthPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
    }

    public override Decimal ModifyDamageAdditive(Creature? target, Decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return this.Owner != dealer || !props.IsPoweredAttack() ? 0M : -this.PassiveVal;
    }
}