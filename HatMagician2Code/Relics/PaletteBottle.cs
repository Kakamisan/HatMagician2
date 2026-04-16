using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Monsters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class PaletteBottle : HatMagician2Relic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    public override bool AddsPet => true;

    // 战斗中绘色数量
    public int RedVal = 0;
    public int BluedVal = 0;
    public int YellowVal = 0;
    
    // public override async Task AfterObtained()
    // {
    //     PaletteBottle paletteBottle = this;
    //     await paletteBottle.SummonPet();
    // }

    public override async Task BeforeCombatStart()
    {
        RedVal = 3;
        BluedVal = 3;
        YellowVal = 3;
        await SummonPet();
    }

    private async Task SummonPet()
    {
        Creature creature = await PlayerCmd.AddPet<BrandColorPetRed>(Owner);
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        RedVal = 0;
        BluedVal = 0;
        YellowVal = 0;
        return Task.CompletedTask;
    }
}