using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SetToFreeThisCombat))]
public class SetToFreeThisCombatPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance)
    {
        if (__instance is HatMagician2Card card)
        {
            card.SetToFreeThisCombatForBrandColor();
        }
    }
}