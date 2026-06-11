using HarmonyLib;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Damage))]
[HarmonyPatch([typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel)])]
public class SingleDamageHookPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        ref Task<IEnumerable<DamageResult>> __result,
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        // Log.Info("[   Hat2   ]------SingleDamageHookPatch------");
        __result = PostfixSub(__result, choiceContext, targets, amount, props, dealer, cardSource);
    }

    private static async Task<IEnumerable<DamageResult>> PostfixSub(Task<IEnumerable<DamageResult>> originTask,
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        var taskResult = await originTask;
        var targets2 = targets.ToList();
        await HatMagician2Mgr.AfterSingleDamageReceived(choiceContext, targets2[0].CombatState, targets2, props, dealer, cardSource);
        return taskResult;
    }
}