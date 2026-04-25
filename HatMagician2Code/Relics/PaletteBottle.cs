using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Monsters;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
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
    public Dictionary<BrandColor, int> BrandColorEnergyMap = new()
    {
        { BrandColor.Red, 0 }, { BrandColor.Blue, 0 }, { BrandColor.Yellow, 0 },
        { BrandColor.Purple, 0 }, { BrandColor.Orange, 0 }, { BrandColor.White, 0 }
    };

    // 绘色场景的实例
    public Dictionary<BrandColor, BattleBrandColorPet?> PetVisuals = new();

    public override async Task BeforeCombatStart()
    {
        this.BrandColorEnergyMap[BrandColor.Red] = 3;
        this.BrandColorEnergyMap[BrandColor.Blue] = 3;
        this.BrandColorEnergyMap[BrandColor.Yellow] = 3;
        this.BrandColorEnergyMap[BrandColor.Purple] = 0;
        this.BrandColorEnergyMap[BrandColor.Orange] = 0;
        this.BrandColorEnergyMap[BrandColor.White] = 0;
        await SummonPet();
    }

    private async Task SummonPet()
    {
        await this.SummonPet<BrandColorPetRed>(BrandColor.Red);
        await this.SummonPet<BrandColorPetBlue>(BrandColor.Blue);
        await this.SummonPet<BrandColorPetYellow>(BrandColor.Yellow);
        await this.SummonPet<BrandColorPetPurple>(BrandColor.Purple);
        await this.SummonPet<BrandColorPetOrange>(BrandColor.Orange);
        await this.SummonPet<BrandColorPetWhite>(BrandColor.White);

        this.UpdateAllPet();
    }

    private async Task SummonPet<T>(BrandColor color) where T : MonsterModel
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
    public void SpendEnergy(BrandColor color, int cost)
    {
        if (color == BrandColor.None || cost <= 0)
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
    public void AddEnergy(BrandColor color, int amount)
    {
        if (color == BrandColor.None || amount <= 0)
            return;

        int current = this.BrandColorEnergyMap.GetValueOrDefault(color, 0);
        this.BrandColorEnergyMap[color] = current + amount;

        // 更新对应颜色的宠物显示
        if (this.PetVisuals.TryGetValue(color, out var pet) && pet != null)
        {
            pet.SetEnergy(this.BrandColorEnergyMap[color]);
        }
    }

    public static async Task AddEnergy(Player player, int amount, BrandColor color = BrandColor.Any)
    {
        var relic = player.GetRelic<PaletteBottle>();
        if (relic == null)
            return;
        var applyColor = color;
        if (applyColor == BrandColor.Any)
        {
            applyColor = (BrandColor)(new Random().Next((int)BrandColor.Red, (int)BrandColor.White));
            relic.AddEnergy(applyColor, amount);
            return;
        }

        if (applyColor == BrandColor.All)
        {
            var c = BrandColor.Red;
            while (c < BrandColor.Rainbow)
            {
                relic.AddEnergy(c, amount);
                c++;
            }
            return;
        }
        
        relic.AddEnergy(applyColor, amount);
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