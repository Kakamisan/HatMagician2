using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandWhitePower : BrandPower
{
    public BrandWhitePower()
    {
        BaseBrandColor = BrandColor.White;
        BasePassiveVal = 1;
        BaseEvokeVal = 2; // 抽牌数
        BaseFusionVal = 1;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new BlockVar(5, ValueProp.Unpowered)); // 格挡数
    
    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        if (this.Applier?.Player == null)
            return;
        // VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_attack_lightning");
        await base.OnEvoke(card);
        await CreatureCmd.GainBlock(this.Applier, this.DynamicVars.Block, null);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), this.EvokeVal, this.Applier.Player);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        await base.OnFusion(cardSource);
        if (this.Applier?.Player == null)
            return;
        await PaletteBottle.AddEnergy(this.Applier.Player, (int)this.FusionVal, this.BaseBrandColor);
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != this.Owner.Side)
            return;
        if (this.Owner.CombatState == null)
            return;
        if (this.Applier?.Player == null)
            return;
        await base.OnPassive();
        await PaletteBottle.AddEnergy(this.Applier.Player, 1);
    }
}