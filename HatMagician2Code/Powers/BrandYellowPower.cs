using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandYellowPower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    protected override decimal BasePassiveVal => 3;
    protected override decimal BaseEvokeVal => 8;
    protected override decimal BaseFusionVal => 0;

    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";
    protected override string EvokeSfx => "event:/sfx/characters/defect/defect_lightning_evoke";
    protected override string ChannelSfx => "event:/sfx/characters/defect/defect_lightning_channel";

    // protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new PowerVar<VulnerablePower>(1));
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Chain)];

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        await base.OnEvoke(card);
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        await BrandPower.ChainDamageCmd(this, this.EvokeVal);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), this.Owner, 1, this.Applier, null);
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        await base.OnPassive();
        if (side != this.Owner.Side)
            return;
        if (this.Owner.CombatState == null)
            return;
        await BrandPower.ChainDamageCmd(this, this.PassiveVal);
    }
}