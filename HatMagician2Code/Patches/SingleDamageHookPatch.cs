using HarmonyLib;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Damage))]
[HarmonyPatch([typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay)])]
public class SingleDamageHookPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        ref Task<IEnumerable<DamageResult>> __result,
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
        )
    {
        // Log.Info("[   Hat2   ]------SingleDamageHookPatch------");
        __result = PostfixSub(__result, choiceContext, targets, amount, props, dealer, cardSource, cardPlay);
    }

    private static async Task<IEnumerable<DamageResult>> PostfixSub(Task<IEnumerable<DamageResult>> originTask,
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
        )
    {
        var taskResult = await originTask;
        var targets2 = targets.ToList();
        if (targets2.Count > 0)
            await HatMagician2Mgr.AfterSingleDamageReceived(choiceContext, targets2[0].CombatState, targets2, props, dealer, cardSource, cardPlay);
        return taskResult;
    }
}