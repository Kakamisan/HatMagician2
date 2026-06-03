using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class PurplePainting : HatMagician2Relic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new Hat2Var(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GloomyPower>()];

    public override async Task BeforeCombatStartLate()
    {
        if (this.Owner.Creature.CombatState == null) return;
        this.Flash();
        var enemies = this.Owner.Creature.CombatState.HittableEnemies;
        await PowerCmd.Apply<GloomyPower>(new ThrowingPlayerChoiceContext(), enemies, this.DynamicHat2Var.IntValue, this.Owner.Creature, null);

        await base.BeforeCombatStartLate();
    }
}