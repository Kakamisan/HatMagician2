using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
public class SpendResourcesPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance)
    {
        if (__instance is HatMagician2Card card)
        {
            PaletteBottle? relic = card.Owner.GetRelic<PaletteBottle>();
            if (relic != null)
            {
                relic.SpendEnergy(card.BaseBrandColor, card.GetBrandColorCostWithModifiers());
            }
        }
    }
}