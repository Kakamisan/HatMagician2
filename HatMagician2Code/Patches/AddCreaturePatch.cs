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
    /// ! 已弃用 !
    /// 在AddCreature执行后：
    /// - BrandColorPet使用固定的基础偏移位置（相当于index=0的位置，不受多宠物间距影响）
    /// - 其他宠物按原逻辑计算偏移，但不把BrandColorPet计入数量
    /// </summary>
    [HarmonyPostfix]
    public static void Postfix(NCombatRoom __instance, Creature creature)
    {
    }
}