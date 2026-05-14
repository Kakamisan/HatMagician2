using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
public static class HasEnoughResourcesForPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, PlayerCombatState __instance, CardModel card,
        ref UnplayableReason reason)
    {
        if (card is HatMagician2Card card2)
        {
            //Log.Info("[   Hat2   ] HasEnoughResourcesForPatch");
            int num1 = Math.Max(0, card2.GetBrandColorCostWithModifiers());
            Player player = card2.Owner;
            BrandColorEnergyState state = BrandColorEnergyMgr.Instance.GetState(player);
            if (num1 > state.BrandColorEnergyMap.GetValueOrDefault(card2.BaseBrandColor))
                reason |= UnplayableReason.StarCostTooHigh;
            __result = reason == UnplayableReason.None;
        }
    }
}