using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Apply))]
[HarmonyPatch([typeof(PlayerChoiceContext), typeof(PowerModel), typeof(Creature), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool)])]
public class PowerCmdApplyPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref Task __result, PlayerChoiceContext choiceContext, PowerModel power, Creature target, decimal amount, Creature? applier, CardModel? cardSource
        , out (BrandColor c1, BrandColor c2) __state)
    {
        // 覆盖旧印记 先判断叠色等
        if (power is BrandPower power2 && target.Powers.FirstOrDefault(p => p is BrandPower) is BrandPower oldPower)
        {
            // 实际应用的印记颜色
            var color = power2.BaseBrandColor;
            HatMagician2Card? card = cardSource as HatMagician2Card;
            var applyColor = color;
            // 叠色
            if ((oldPower.BaseBrandColor & color) == 0 && (color & (color - 1)) == 0 && (card == null || !card.Keywords.Contains(HatMagician2Keywords.Erosion)))
            {
                applyColor = oldPower.BaseBrandColor | color;
            }
            
            __result = PrefixSub(applyColor, cardSource, oldPower, choiceContext, target, applier);
            __state = (color, applyColor);
            return false;
        }

        // 应用新印记 直接原逻辑
        if (power is BrandPower power3)
        {
            __state = (power3.BaseBrandColor, power3.BaseBrandColor);
            return true;
        }

        __state = (BrandColor.None, BrandColor.None);
        return true;
    }

    private static async Task PrefixSub(BrandColor applyColor, CardModel? cardSource, BrandPower oldPower, PlayerChoiceContext choiceContext, Creature target, Creature? applier)
    {
        // 触发刻印效果
        HatMagician2Card? card = cardSource as HatMagician2Card;
        await oldPower.OnEvokePublic(card);
        await PowerCmd.Remove(oldPower);

        // 应用新印记
        switch (applyColor)
        {
            case BrandColor.Red:
                await PowerCmd.Apply<BrandRedPower>(choiceContext, target, 1, applier, cardSource, true);
                break;
            case BrandColor.Yellow:
                await PowerCmd.Apply<BrandYellowPower>(choiceContext, target, 1, applier, cardSource, true);
                break;
            case BrandColor.Blue:
                await PowerCmd.Apply<BrandBluePower>(choiceContext, target, 1, applier, cardSource, true);
                break;
            case BrandColor.Purple:
                await PowerCmd.Apply<BrandPurplePower>(choiceContext, target, 1, applier, cardSource, true);
                break;
            case BrandColor.Orange:
                await PowerCmd.Apply<BrandOrangePower>(choiceContext, target, 1, applier, cardSource, true);
                break;
            case BrandColor.White:
                await PowerCmd.Apply<BrandWhitePower>(choiceContext, target, 1, applier, cardSource, true);
                break;
            case BrandColor.Rainbow:
                await PowerCmd.Apply<BrandRainbowPower>(choiceContext, target, 1, applier, cardSource, true);
                break;
            case BrandColor.None:
            case BrandColor.Any:
            case BrandColor.All:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [HarmonyPostfix]
    public static void Postfix(ref Task __result, PlayerChoiceContext choiceContext, PowerModel power, Creature target, decimal amount, Creature? applier, CardModel? cardSource
        , (BrandColor c1, BrandColor c2) __state)
    {
        if (power is BrandPower power2 && __state.c1 != BrandColor.None)
        {
            var color = __state.c1;
            var applyColor = __state.c2;
            __result = PostfixSub(__result, cardSource, target, power2, color, applyColor);
        }
    }

    private static async Task PostfixSub(Task originTask, CardModel? cardSource, Creature target, BrandPower power2, BrandColor color, BrandColor applyColor)
    {
        await originTask;
        var card = cardSource as HatMagician2Card;
        var newPower = (BrandPower?)target.Powers.FirstOrDefault(p => p is BrandPower);
        var isFusion = color != applyColor;

        if (newPower != null) await newPower.OnApplyPublic(card, isFusion);

        // 其他杂项
        if (card != null) card.IsBrandApplied = true;
    }
}

// 原逻辑判断为叠层
[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.ModifyAmount))]
[HarmonyPatch([typeof(PlayerChoiceContext), typeof(PowerModel), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool)])]
public class PowerCmdModifyAmountPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref Task __result, PlayerChoiceContext choiceContext, PowerModel power, decimal offset, Creature? applier, CardModel? cardSource)
    {
        // 总之印记叠层就是应用新的印记
        if (power is BrandPower power2)
        {
            __result = PrefixSub(cardSource, power2, applier, choiceContext);
            return false;
        }

        return true;
    }

    private static async Task PrefixSub(CardModel? cardSource, BrandPower power2, Creature? applier, PlayerChoiceContext choiceContext)
    {
        var card = cardSource as HatMagician2Card;
        var target = power2.Owner;
        var color = power2.BaseBrandColor;

        // 触发刻印效果
        await power2.OnEvokePublic(card);
        await PowerCmd.Remove(power2);

        await BrandPower.ApplyBrandPower(cardSource, applier ?? cardSource?.Owner.Creature, choiceContext, target, color);
    }
}