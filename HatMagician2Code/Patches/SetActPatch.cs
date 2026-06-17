using System.Runtime.CompilerServices;
using HarmonyLib;
using HatMagician2.HatMagician2Code.Acts;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Monsters;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.SetActInternal))]
public class SetActPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref RunManager __instance, int actIndex)
    {
        ref RunState? runState = ref RunState(__instance);
        if (runState != null && actIndex == 2 && ColorFinderEncounter.IsValidForAct(runState) && __instance.AscensionManager.HasLevel(AscensionLevel.DoubleBoss))
        {
            // 修改第三幕为画界 第二个BOSS固定为寻色者
            ref var acts = ref RunActs(runState);
            var list = acts.ToList();
            var act = ModelDb.Act<ActPaintingWorld>().ToMutable();

            act.GenerateRooms(runState.Rng.UpFront, runState.UnlockState, runState.Players.Count > 1);
            // if (__instance.ShouldApplyTutorialModifications())
            //     act.ApplyDiscoveryOrderModifications(runState.UnlockState);
            EncounterModel encounter = ModelDb.Encounter<ColorFinderEncounter>();
            act.SetSecondBossEncounter(encounter);

            list[actIndex] = act;
            acts = list;
        }

        return true;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<State>k__BackingField")]
    private static extern ref RunState? RunState(RunManager target);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Acts>k__BackingField")]
    private static extern ref IReadOnlyList<ActModel> RunActs(RunState target);
}