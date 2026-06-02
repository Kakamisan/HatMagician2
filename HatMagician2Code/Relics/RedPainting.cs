using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class RedPainting : HatMagician2Relic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeCombatStartLate()
    {
        if (this.Owner.Creature.CombatState == null) return;
        this.Flash();
        var enemies = this.Owner.Creature.CombatState.HittableEnemies;
        var enemy = this.Owner.RunState.Rng.CombatTargets.NextItem(enemies);
        if (enemy != null)
        {
            await BrandPower.ApplyBrandPower(null, this.Owner.Creature, new ThrowingPlayerChoiceContext(), enemy, BrandColor.Red);
        }

        await base.BeforeCombatStartLate();
    }
}