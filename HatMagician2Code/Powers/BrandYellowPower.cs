using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandYellowPower : BrandPower
{
    public BrandYellowPower()
    {
        BaseBrandColor = HatMagician2BrandColor.Yellow;
        BasePassiveVal = 3;
        BaseEvokeVal = 5;
    }

    protected override string EvokeSfx => "event:/sfx/characters/defect/defect_lightning_evoke";

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        BrandYellowPower power = this;
        if (!this.Owner.IsAlive)
            return;
        if (power.Owner.CombatState == null)
            return;
        await base.OnEvoke(card);
        var enemies = power.Owner.CombatState.HittableEnemies.Where(c => c.Powers.Any(p => p is BrandPower)).ToList();
        IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemies,
            power.EvokeVal, ValueProp.Unpowered, null, null);
        foreach (var target1 in (IEnumerable<Creature>)enemies)
            VfxCmd.PlayOnCreature(target1, "vfx/vfx_attack_lightning");
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        BrandYellowPower power = this;
        if (side != power.Owner.Side)
            return;
        if (power.Owner.CombatState == null)
            return;
        this.OnSfx();
        var enemies = power.Owner.CombatState.HittableEnemies.Where(c => c.Powers.Any(p => p is BrandPower)).ToList();
        IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemies,
            power.PassiveVal, ValueProp.Unpowered, null, null);
        foreach (var target1 in (IEnumerable<Creature>)enemies)
            VfxCmd.PlayOnCreature(target1, "vfx/vfx_attack_lightning");
        await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
}