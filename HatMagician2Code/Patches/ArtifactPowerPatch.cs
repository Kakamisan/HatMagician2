using HarmonyLib;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(ArtifactPower), nameof(ArtifactPower.TryModifyPowerAmountReceived))]
public class ArtifactPowerPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, PowerModel canonicalPower, Decimal amount, ref Decimal modifiedAmount)
    {
        if (canonicalPower is HatMagician2Power { FakeDebuff: true })
        {
            modifiedAmount = amount;
            __result = false;
        }
    }
}