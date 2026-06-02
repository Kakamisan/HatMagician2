using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(BrilliantScarf), nameof(BrilliantScarf.TryModifyEnergyCostInCombatLate))]
public class BrilliantScarfPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel card, ref bool __result)
    {
        if (card is HatMagician2Card card2)
        {
            card2.SetDynamicFreeBrandColorCost(0, __result);
        }
    }
}

[HarmonyPatch(typeof(VoidFormPower), nameof(VoidFormPower.TryModifyEnergyCostInCombatLate))]
public class VoidFormPowerPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel card, ref bool __result)
    {
        if (card is HatMagician2Card card2)
        {
            card2.SetDynamicFreeBrandColorCost(1, __result);
        }
    }
}