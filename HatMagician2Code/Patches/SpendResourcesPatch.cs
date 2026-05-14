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
    public static void Postfix(CardModel __instance)
    {
        if (__instance is HatMagician2Card card)
        {
            // BrandColorEnergyState state = BrandColorEnergyMgr.Instance.GetState(card.Owner);
            // state.SpendEnergy(card.BaseBrandColor, card.GetBrandColorCostWithModifiers());
            card.SpendEnergy();
        }
    }
}