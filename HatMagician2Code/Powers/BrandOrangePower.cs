using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandOrangePower : BrandPower
{
    public BrandOrangePower()
    {
        BaseBrandColor = BrandColor.Orange;
        BasePassiveVal = 6;
        BaseEvokeVal = 1;
        BaseFusionVal = 8;
    }
    
    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Chain)];
    
    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        await base.OnEvoke(card);
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        // 下次攻击对其他目标造成等量伤害
        await PowerCmd.Apply<ChainDamagePower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, this.Applier, null);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        await base.OnFusion(cardSource);
        if (this.Applier?.Player == null)
            return;
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, 1, this.BaseBrandColor);
        await BrandPower.ChainDamageCmd(this, this.FusionVal);
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