using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.AutoPlay))]
public class AutoPlayXValuePatch
{
    [HarmonyPrefix]
    public static void Prefix(CardModel card)
    {
        if (card is HatMagician2Card { HasBrandColorCostX: true, BaseBrandColor: > BrandColor.None and < BrandColor.Rainbow } card2 && HatMagician2Mgr.Instance != null)
        {
            card2.LastBrandColorCost = HatMagician2Mgr.Instance.GetState(card.Owner).GetEnergy(card2.BaseBrandColor);
        }
    }
}