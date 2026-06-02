using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandOrangePower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.Orange;
    protected override decimal BasePassiveVal => 6;
    protected override decimal BaseEvokeVal => 1;
    protected override decimal BaseEvokeVal2 => 1;
    protected override decimal BaseFusionVal => 8;
    protected override decimal BaseFusionVal2 => 1;

    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Chain)];

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (!this.Owner.IsAlive) return;
        if (this.Owner.CombatState == null) return;
        await base.OnEvoke(card);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal2, this.Applier, null);
        // 下次攻击对其他目标造成等量伤害
        await PowerCmd.Apply<ChainDamagePower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        if (this.IsOnFusionEd) return;
        if (this.Applier?.Player == null) return;
        await base.OnFusion(cardSource);
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, (int)this.FusionVal2, this.BaseBrandColor);
        await BrandPower.ChainDamageCmd(this, this.FusionVal);
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != this.Owner.Side) return;
        await this.OnPassive();
        await base.AfterSideTurnStart(side, participants, combatState);
    }

    protected override async Task OnPassive(bool setFlag = true)
    {
        if (this.Owner.CombatState == null) return;
        await base.OnPassive(setFlag);
        await UsePassive(this);
    }

    public static async Task UsePassive(BrandPower power, CardModel? card = null, int cnt = 1)
    {
        await BrandPower.ChainDamageCmd(power.Owner, power.PassiveVal, card != null ? card.Owner.Creature : power.Applier, card, true, cnt);
        await Task.CompletedTask;
    }
}