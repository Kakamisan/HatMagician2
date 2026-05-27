using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
public class SpendResourcesPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref Task<(int, int)> __result, CardModel __instance)
    {
        if (__instance is HatMagician2Card card)
        {
            // card.SpendEnergy();
            __result = PostfixSub(__result, card);
        }
    }

    private static async Task<(int, int)> PostfixSub(Task<(int, int)> originTask, HatMagician2Card card)
    {
        var result = await originTask;
        await card.SpendEnergy();
        return result;
    }
}