using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class PaletteBottle : HatMagician2Relic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStartLate()
    {
        this.Flash();
        await HatMagician2Mgr.AddEnergy(this.Owner, 3, BrandColor.Red);
        await HatMagician2Mgr.AddEnergy(this.Owner, 3, BrandColor.Yellow);
        await HatMagician2Mgr.AddEnergy(this.Owner, 3, BrandColor.Blue);
        await base.BeforeCombatStartLate();
    }

    // 升级遗物
    public override PaletteBox? GetUpgradeReplacement() => ModelDb.Relic<PaletteBox>();
}