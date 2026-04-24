using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Monsters;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class PaletteBottle : HatMagician2Relic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override bool AddsPet => true;

    // 绘色能量
    public Dictionary<HatMagician2BrandColor, int> BrandColorEnergyMap = new()
    {
        { HatMagician2BrandColor.Red, 0 }, { HatMagician2BrandColor.Blue, 0 }, { HatMagician2BrandColor.Yellow, 0 },
        { HatMagician2BrandColor.Purple, 0 }, { HatMagician2BrandColor.Orange, 0 }, { HatMagician2BrandColor.White, 0 }
    };

    // 绘色场景的实例
    public Dictionary<HatMagician2BrandColor, BattleBrandColorPet?> PetVisuals = new();

    public override async Task BeforeCombatStart()
    {
        this.BrandColorEnergyMap[HatMagician2BrandColor.Red] = 3;
        this.BrandColorEnergyMap[HatMagician2BrandColor.Blue] = 3;
        this.BrandColorEnergyMap[HatMagician2BrandColor.Yellow] = 3;
        await SummonPet();
    }

    private async Task SummonPet()
    {
        await this.SummonPet<BrandColorPetRed>(HatMagician2BrandColor.Red);
        await this.SummonPet<BrandColorPetBlue>(HatMagician2BrandColor.Blue);
        await this.SummonPet<BrandColorPetYellow>(HatMagician2BrandColor.Yellow);
        await this.SummonPet<BrandColorPetPurple>(HatMagician2BrandColor.Purple);
        await this.SummonPet<BrandColorPetOrange>(HatMagician2BrandColor.Orange);
        await this.SummonPet<BrandColorPetWhite>(HatMagician2BrandColor.White);

        this.UpdateAllPet();
    }

    private async Task SummonPet<T>(HatMagician2BrandColor color) where T : MonsterModel
    {
        Creature creature = await PlayerCmd.AddPet<T>(Owner);
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
        BattleBrandColorPet? pet = creatureNode?.Visuals as BattleBrandColorPet;
        this.PetVisuals[color] = pet;
    }

    // 更新所有绘色显示
    private void UpdateAllPet()
    {
        foreach (var key in this.PetVisuals.Keys)
        {
            int energy = this.BrandColorEnergyMap.GetValueOrDefault(key, 0);
            this.PetVisuals[key]?.SetEnergy(energy);
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        foreach (var key in this.BrandColorEnergyMap.Keys)
        {
            this.BrandColorEnergyMap[key] = 0;
        }

        this.PetVisuals.Clear();
        return Task.CompletedTask;
    }

    // 消耗绘色能量
    public void SpendEnergy(HatMagician2BrandColor color, int cost)
    {
        if (color == HatMagician2BrandColor.None || cost <= 0)
            return;

        int current = this.BrandColorEnergyMap.GetValueOrDefault(color, 0);
        this.BrandColorEnergyMap[color] = Math.Max(0, current - cost);

        // 更新对应颜色的宠物显示
        if (this.PetVisuals.TryGetValue(color, out var pet) && pet != null)
        {
            pet.SetEnergy(this.BrandColorEnergyMap[color]);
        }
    }

    // 增加绘色能量
    public void AddEnergy(HatMagician2BrandColor color, int amount)
    {
        if (color == HatMagician2BrandColor.None || amount <= 0)
            return;

        int current = this.BrandColorEnergyMap.GetValueOrDefault(color, 0);
        this.BrandColorEnergyMap[color] = current + amount;

        // 更新对应颜色的宠物显示
        if (this.PetVisuals.TryGetValue(color, out var pet) && pet != null)
        {
            pet.SetEnergy(this.BrandColorEnergyMap[color]);
        }
    }

    public static Decimal ModifyBrandColorCost(
        ICombatState combatState,
        HatMagician2Card card,
        Decimal originalCost)
    {
        if (originalCost < 0M)
            return originalCost;
        Decimal modifiedCost = originalCost;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyBrandColorCost(card, modifiedCost, out modifiedCost);
            }
        }

        return modifiedCost;
    }
}