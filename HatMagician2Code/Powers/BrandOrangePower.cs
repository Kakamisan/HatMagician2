using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandOrangePower : BrandPower
{
    public BrandOrangePower()
    {
        BaseBrandColor = BrandColor.Orange;
        BasePassiveVal = 6;
        BaseEvokeVal = 1;
        BaseFusionVal = 1;
    }
    
    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";
    
    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (!this.Owner.IsAlive)
            return;
        if (this.Owner.CombatState == null)
            return;
        VfxCmd.PlayOnCreature(this.Owner, "vfx/vfx_attack_lightning");
        await base.OnEvoke(card);
        // todo 下次攻击对其他目标造成等量伤害
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
        var enemies = this.Owner.CombatState.HittableEnemies.Where(c => c.Powers.Any(p => p is BrandPower)).ToList();
        foreach (var target1 in (IEnumerable<Creature>)enemies)
            VfxCmd.PlayOnCreature(target1, "vfx/vfx_attack_lightning");
        await base.OnPassive();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemies, this.PassiveVal, ValueProp.Unpowered, this.Applier, null);
        // await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
}