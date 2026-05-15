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
    public override BrandColor BaseBrandColor => BrandColor.Blue;
    protected override decimal BasePassiveVal => 2;
    protected override decimal BaseEvokeVal => 3;
    protected override decimal BaseFusionVal => 0;

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
        await PowerCmd.Apply<FreezeStrengthPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
    }

    public override Decimal ModifyDamageAdditive(Creature? target, Decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return this.Owner != dealer || !props.IsPoweredAttack() ? 0M : -this.PassiveVal;
    }
}