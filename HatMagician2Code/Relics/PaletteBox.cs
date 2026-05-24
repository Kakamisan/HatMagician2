using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class PaletteBox() : HatMagician2Relic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStartLate()
    {
        this.Flash();
        await HatMagician2Mgr.AddEnergy(this.Owner, 4, BrandColor.Red);
        await HatMagician2Mgr.AddEnergy(this.Owner, 4, BrandColor.Yellow);
        await HatMagician2Mgr.AddEnergy(this.Owner, 4, BrandColor.Blue);
        await HatMagician2Mgr.AddEnergy(this.Owner, 2, BrandColor.Purple);
        await HatMagician2Mgr.AddEnergy(this.Owner, 2, BrandColor.Orange);
        await HatMagician2Mgr.AddEnergy(this.Owner, 2, BrandColor.White);
        await base.BeforeCombatStartLate();
    }
}