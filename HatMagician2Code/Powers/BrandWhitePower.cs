using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandWhitePower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    protected override decimal BasePassiveVal => 1;
    protected override decimal BaseEvokeVal => 2;
    protected override decimal BaseFusionVal => 6;

    // protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new BlockVar(5, ValueProp.Unpowered)); // 格挡数
    
    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        await base.OnEvoke(card);
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        if (this.Applier?.Player == null)
            return;
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), this.EvokeVal, this.Applier.Player);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        await base.OnFusion(cardSource);
        if (this.Applier?.Player == null)
            return;
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, 1, this.BaseBrandColor);
        await CreatureCmd.GainBlock(this.Applier, new BlockVar(this.FusionVal, ValueProp.Unpowered), null);
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        await base.OnPassive();
        if (side != this.Owner.Side)
            return;
        if (this.Owner.CombatState == null)
            return;
        if (this.Applier?.Player == null)
            return;
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, (int)this.PassiveVal);
    }
}