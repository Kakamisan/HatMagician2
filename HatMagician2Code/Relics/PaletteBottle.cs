using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class PaletteBottle : HatMagician2Relic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override Task BeforeCombatStartLate()
    {
        _ = HatMagician2Mgr.AddEnergy(this.Owner, 3, BrandColor.Red);
        _ = HatMagician2Mgr.AddEnergy(this.Owner, 3, BrandColor.Yellow);
        _ = HatMagician2Mgr.AddEnergy(this.Owner, 3, BrandColor.Blue);
        return base.BeforeCombatStartLate();
    }
}