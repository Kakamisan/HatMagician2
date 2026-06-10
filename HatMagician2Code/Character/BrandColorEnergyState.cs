using Godot;
using HatMagician2.HatMagician2Code.SceneControl;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HatMagician2.HatMagician2Code.Character;

public class BrandColorEnergyState(Player player)
{
    private bool _summon;

    // 绘色能量
    public readonly Dictionary<BrandColor, int> BrandColorEnergyMap = new()
    {
        { BrandColor.Red, 0 }, { BrandColor.Blue, 0 }, { BrandColor.Yellow, 0 },
        { BrandColor.Purple, 0 }, { BrandColor.Orange, 0 }, { BrandColor.White, 0 }
    };

    // 绘色场景的实例
    private readonly Dictionary<BrandColor, BattleBrandColorPet?> _petVisuals = new();

    public void BeforeCombatStart()
    {
        foreach (var key in this.BrandColorEnergyMap.Keys)
        {
            this.BrandColorEnergyMap[key] = 0;
        }

        this._summon = false;

        //this.UpdateAllPet();
    }

    // 添加一个绘色显示
    private void SummonBrandColor(BrandColor color)
    {
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (creatureNode == null)
            return;
        BattleBrandColorPet? pet = BattleBrandColorPet.Create(creatureNode, color);
        if (pet == null)
            return;
        this._petVisuals[color] = pet;
        pet.SetEnergy(0, false);
    }

    // 隐藏一个绘色
    private void HideBrandColor(BrandColor color)
    {
        this._petVisuals.TryGetValue(color, out var pet);
        if (pet != null)
        {
            pet.Visible = false;
        }
    }

    // 召唤全部
    private void SummonAll()
    {
        if (this._summon) return;
        this._summon = true;
        foreach (var color in this.BrandColorEnergyMap.Keys)
        {
            this.SummonBrandColor(color);
        }
    }

    // 寄了之后隐藏全部绘色
    public void HideAll()
    {
        if (!this._summon) return;
        foreach (var color in this.BrandColorEnergyMap.Keys)
        {
            this.HideBrandColor(color);
        }
    }

    // 更新所有绘色显示
    // private void UpdateAllPet()
    // {
    //     foreach (var key in this._petVisuals.Keys)
    //     {
    //         int energy = this.BrandColorEnergyMap.GetValueOrDefault(key, 0);
    //         var visual = this._petVisuals[key];
    //         if (visual != null && GodotObject.IsInstanceIdValid(visual.GetInstanceId()))
    //         {
    //             visual.SetEnergy(energy);
    //         }
    //     }
    //
    //     //SfxCmd.Play("event:/sfx/ui/gain_energy");
    // }

    // 消耗绘色能量
    public async Task SpendEnergy(BrandColor color, int cost)
    {
        if (color == BrandColor.None || cost <= 0)
            return;

        int current = this.BrandColorEnergyMap.GetValueOrDefault(color, 0);
        this.BrandColorEnergyMap[color] = Math.Max(0, current - cost);

        await HatMagician2Mgr.AfterSpendEnergy(player, cost, color);

        // 更新对应颜色的宠物显示
        if (this._petVisuals.TryGetValue(color, out var pet) && pet != null)
        {
            pet.SetEnergy(this.BrandColorEnergyMap[color]);
        }
    }

    // 增加绘色能量
    public async Task AddEnergy(BrandColor color, int amount)
    {
        if (color == BrandColor.None || amount <= 0)
            return;

        // 添加能量时召唤绘色显示
        //this.SummonBrandColor(color);
        this.SummonAll();

        int current = this.BrandColorEnergyMap.GetValueOrDefault(color, 0);
        this.BrandColorEnergyMap[color] = current + amount;

        await HatMagician2Mgr.AfterAddEnergy(player, amount, color);

        // 更新对应颜色的宠物显示
        if (this._petVisuals.TryGetValue(color, out var pet) && pet != null)
        {
            pet.SetEnergy(this.BrandColorEnergyMap[color]);
            SfxCmd.Play("event:/sfx/ui/gain_energy");
        }
    }

    // 获取当前绘色的值
    public int GetEnergy(BrandColor color)
    {
        return color is not (> BrandColor.None and < BrandColor.Rainbow) ? 0 : this.BrandColorEnergyMap[color];
    }

    // 获取当前拥有的绘色种类
    public int GetBrandColorTypeCnt()
    {
        return BrandColorEnergyMap.Values.Count(energy => energy > 0);
    }
}