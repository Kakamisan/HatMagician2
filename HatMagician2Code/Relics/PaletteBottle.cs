using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.SceneControl;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
    public readonly Dictionary<BrandColor, int> BrandColorEnergyMap = new()
    {
        { BrandColor.Red, 0 }, { BrandColor.Blue, 0 }, { BrandColor.Yellow, 0 },
        { BrandColor.Purple, 0 }, { BrandColor.Orange, 0 }, { BrandColor.White, 0 }
    };

    // 绘色场景的实例
    public readonly Dictionary<BrandColor, BattleBrandColorPet?> PetVisuals = new();

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
        await this.SummonBrandColor(BrandColor.Red);
        await this.SummonBrandColor(BrandColor.Blue);
        await this.SummonBrandColor(BrandColor.Yellow);
        await this.SummonBrandColor(BrandColor.Purple);
        await this.SummonBrandColor(BrandColor.Orange);
        await this.SummonBrandColor(BrandColor.White);

        this.UpdateAllPet();
    }

    private async Task SummonBrandColor(BrandColor color)
    {
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(this.Owner.Creature);
        if (creatureNode == null)
            return;
        BattleBrandColorPet? pet = BattleBrandColorPet.Create(creatureNode, color);
        if (pet == null)
            return;
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

        SfxCmd.Play("event:/sfx/ui/gain_energy");
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
            SfxCmd.Play("event:/sfx/ui/gain_energy");
        }
    }

    public static async Task AddEnergy(Player player, int amount, BrandColor color = BrandColor.Any)
    {
        var relic = player.GetRelic<PaletteBottle>();
        if (relic == null) return;
        var applyColor = color;
        if (applyColor == BrandColor.Any)
        {
            applyColor = (BrandColor)player.RunState.Rng.CombatEnergyCosts.NextInt((int)BrandColor.Red, (int)BrandColor.White);
            relic.AddEnergy(applyColor, amount);
            return;
        }

        if (applyColor is BrandColor.All or BrandColor.Rainbow)
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