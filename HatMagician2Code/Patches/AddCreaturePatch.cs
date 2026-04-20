using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using HatMagician2.HatMagician2Code.Monsters;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HatMagician2.HatMagician2Code.Patches;

/// <summary>
/// 修改NCombatRoom.AddCreature方法，使BrandColorPet不受多宠物时的额外位置偏移影响，
/// 但保留与玩家之间的基础偏移（-20, +10）。
/// 同时确保BrandColorPet的数量不影响其他宠物的正常偏移计算
/// </summary>
[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
public static class AddCreaturePatch
{
    /// <summary>
    /// 在AddCreature执行后：
    /// - BrandColorPet使用固定的基础偏移位置（相当于index=0的位置，不受多宠物间距影响）
    /// - 其他宠物按原逻辑计算偏移，但不把BrandColorPet计入数量
    /// </summary>
    [HarmonyPostfix]
    public static void Postfix(NCombatRoom __instance, Creature creature)
    {
        // 只处理有主人的宠物
        if (creature.PetOwner == null)
            return;

        Player player = creature.PetOwner;
        NCreature? ownerNode = __instance.GetCreatureNode(player.Creature);
        
        if (ownerNode == null)
            return;

        // 获取该玩家的所有宠物节点（排除Osty的特殊处理）
        List<NCreature> allPets = __instance.CreatureNodes
            .Where(c => c.Entity.PetOwner == player)
            .Where(c => !(c.Entity.Monster is Osty) || !LocalContext.IsMe(player))
            .ToList();

        // 分离BrandColorPet和其他宠物
        List<NCreature> brandColorPets = allPets
            .Where(c => c.Entity.Monster is BrandColorPet)
            .ToList();
        
        List<NCreature> otherPets = allPets
            .Where(c => !(c.Entity.Monster is BrandColorPet))
            .ToList();

        // BrandColorPet使用固定的基础偏移位置（相当于只有一个宠物时的位置）
        // 原公式：ownerNode.Position.X - 20.0 + index * spacing + petNode.Visuals.Bounds.Size.X * 0.5
        // 对于BrandColorPet，不使用index * spacing，始终使用index=0的位置
        foreach (var petNode in brandColorPets)
        {
            petNode.Position = new Vector2(
                // (float)(ownerNode.Position.X - 20.0 + petNode.Visuals.Bounds.Size.X * 0.5),
                // ownerNode.Position.Y + 10f
                ownerNode.Position.X,
                ownerNode.Position.Y
            );
            petNode.ToggleIsInteractable(false);
        }

        // 只对非BrandColorPet的宠物重新计算位置偏移（不把BrandColorPet计入数量）
        if (otherPets.Count > 0)
        {
            float spacing = otherPets.Count > 1 
                ? ownerNode.Visuals.Bounds.Size.X / (float)(otherPets.Count - 1) 
                : 0.0f;
            
            for (int index = 0; index < otherPets.Count; index++)
            {
                NCreature petNode = otherPets[index];
                petNode.Position = new Vector2(
                    (float)(ownerNode.Position.X - 20.0 + index * spacing + petNode.Visuals.Bounds.Size.X * 0.5),
                    ownerNode.Position.Y + 10f
                );
                petNode.ToggleIsInteractable(false);
            }
        }

        // 处理暗色调（当玩家位置Y < 199时）
        // NCreature? newCreatureNode = __instance.GetCreatureNode(creature);
        // if (ownerNode.Position.Y < 199.0f && newCreatureNode != null)
        // {
        //     newCreatureNode.Visuals.Modulate = new Color(0.5f, 0.5f, 0.5f);
        // }
    }
}
