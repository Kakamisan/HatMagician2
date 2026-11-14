using HarmonyLib;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
// using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Damage))]
[HarmonyPatch([typeof(PlayerChoiceContext), typeof(IEnumerable<DamageTargetData>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay), typeof(ModifyDamageHookType)])]
public class SingleDamageHookPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        ref Task<IEnumerable<DamageResult>> __result,
        PlayerChoiceContext choiceContext,
        IEnumerable<DamageTargetData>? targetData,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay,
        ModifyDamageHookType modifyDamageHookType
        )
    {
        // Log.Info("[   Hat2   ]------SingleDamageHookPatch------");
        __result = PostfixSub(__result, choiceContext, targetData, amount, props, dealer, cardSource, cardPlay);
    }

    private static async Task<IEnumerable<DamageResult>> PostfixSub(Task<IEnumerable<DamageResult>> originTask,
        PlayerChoiceContext choiceContext,
        IEnumerable<DamageTargetData>? targetData,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
        )
    {
        var taskResult = await originTask;
        var targets2 = targetData?.Select<DamageTargetData, Creature>(data => data.target).ToList();
        if (targets2?.Count > 0)
            await HatMagician2Mgr.AfterSingleDamageReceived(choiceContext, targets2[0].CombatState, targets2, props, dealer, cardSource, cardPlay);
        return taskResult;
    }
}