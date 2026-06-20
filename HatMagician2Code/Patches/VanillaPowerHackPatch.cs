using HarmonyLib;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HatMagician2.HatMagician2Code.Patches;

public class VanillaPowerHackPatch
{
}

[HarmonyPatch(typeof(SurroundedPower), nameof(SurroundedPower.ModifyDamageMultiplicative))]
public class SurroundedPowerHack
{
    [HarmonyPostfix]
    public static void Postfix(ref SurroundedPower __instance, ref decimal __result)
    {
        __result = HatMagician2Mgr.ModifyVanillaPowerHack(__instance, __result);
    }
}

[HarmonyPatch(typeof(PowerModel))]
[HarmonyPatch("get_Description")]
public class PowerModelPatchForSurroundedPower
{
    [HarmonyPostfix]
    public static void Postfix(ref LocString __result, ref PowerModel __instance)
    {
        if (__instance.IsCanonical)
        {
            return;
        }

        if (__instance is SurroundedPower && Hat2CardStateSingleton.GetIsCleanSurroundedPower())
        {
            __result = new LocString("powers", "HATMAGICIAN2-SURROUNDED_POWER_IS_CLEAN.description");
        }
    }
}