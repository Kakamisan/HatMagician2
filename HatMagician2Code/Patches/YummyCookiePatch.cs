using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(YummyCookie))]
[HarmonyPatch("get_IconBaseName")]
public class YummyCookiePatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, ref YummyCookie __instance)
    {
        if (__instance.Owner.Character is Character.HatMagician2)
        {
            __result = "yummy_cookie_hatmagician2";
        }
    }
}