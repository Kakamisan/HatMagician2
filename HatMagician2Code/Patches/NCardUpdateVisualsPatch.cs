using System.Reflection;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace HatMagician2.HatMagician2Code.Patches;

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
public class NCardUpdateVisualsPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCard __instance, PileType pileType, CardPreviewMode previewMode)
    {
        var nodeName = "BrandColorCardIcon";
        Node2D? node = null;
        if (__instance.Body.HasNode(nodeName))
            node = __instance.Body.GetNode<Node2D>(nodeName);
        if (__instance.Model is HatMagician2Card card)
        {
            // 初始化
            if (node == null)
            {
                PackedScene scene = GD.Load<PackedScene>("res://HatMagician2/scenes/brand_color_card_icon.tscn");
                // Log.Info("[    Hat2    ]LoadScene");
                if (scene != null)
                {
                    Node2D? newNode = scene.Instantiate() as Node2D;
                    // Log.Info("[    Hat2    ]Instantiate");
                    if (newNode != null)
                    {
                        newNode.Name = nodeName;
                        TextureRect? star =
                            typeof(NCard).GetField("_starIcon", BindingFlags.NonPublic | BindingFlags.Instance)
                                ?.GetValue(__instance) as TextureRect;
                        // Log.Info("[    Hat2    ]getStar");
                        if (star != null)
                        {
                            // 位置和辉星一致
                            newNode.Position = new Vector2(star.Position.X, star.Position.Y);
                            __instance.Body.AddChild(newNode);
                            var starIdx = star.GetIndex();
                            __instance.Body.MoveChild(newNode, starIdx);
                            node = newNode;

                            // Log.Info("[    Hat2    ]AddChild");
                        }
                    }
                }
            }

            // 更新卡牌绘色icon显示
            if (node != null)
            {
                Control icon = node.GetNode<Control>((NodePath)"%BrandColorIcon");
                MegaLabel label = node.GetNode<MegaLabel>((NodePath)"%BrandColorLabel");
                TextureRect unplayable = node.GetNode<TextureRect>((NodePath)"%UnplayableBrandColorIcon");
                if (icon != null && label != null && unplayable != null)
                {
                    UpdateBrandColorCostVisuals(__instance, pileType, node, label, icon, unplayable, card);
                    // Log.Info("[    Hat2    ]UpdateVisuals");
                }
            }
        }
        else
        {
            // 疑似节点被复用了，所以这里隐藏一下
            if (node != null)
            {
                Control icon = node.GetNode<Control>((NodePath)"%BrandColorIcon");
                if (icon != null)
                    icon.Visible = false;
            }
        }
    }

    public static void UpdateBrandColorCostVisuals(NCard instance, PileType pileType, Node2D node, MegaLabel label,
        Control icon,
        TextureRect unplayable, HatMagician2Card card)
    {
        if (instance.Visibility != ModelVisibility.Visible)
        {
            label.SetTextAutoSize(string.Empty);
            icon.Visible = false;
            label.AddThemeColorOverride(ThemeConstants.Label.FontColor, StsColors.cream);
            label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, card.Pool.EnergyOutlineColor);
        }
        else
        {
            // 打开对应的绘色节点
            node.GetNode<TextureRect>((NodePath)"%BrandColorRed").Visible =
                card.BaseBrandColor == BrandColor.Red;
            node.GetNode<TextureRect>((NodePath)"%BrandColorBlue").Visible =
                card.BaseBrandColor == BrandColor.Blue;
            node.GetNode<TextureRect>((NodePath)"%BrandColorYellow").Visible =
                card.BaseBrandColor == BrandColor.Yellow;
            node.GetNode<TextureRect>((NodePath)"%BrandColorOrange").Visible =
                card.BaseBrandColor == BrandColor.Orange;
            node.GetNode<TextureRect>((NodePath)"%BrandColorPurple").Visible =
                card.BaseBrandColor == BrandColor.Purple;
            node.GetNode<TextureRect>((NodePath)"%BrandColorWhite").Visible =
                card.BaseBrandColor == BrandColor.White;

            if (card.HasBrandColorCostX)
            {
                label.SetTextAutoSize("X");
                icon.Visible = true;
            }
            else
            {
                var cost = card.GetBrandColorCostWithModifiers();
                label.SetTextAutoSize(cost.ToString());
                icon.Visible = cost >= 0;
            }

            UpdateBrandColorCostColor(instance, pileType, label, card);
            UnplayableReason reason;
            if (pileType == PileType.Hand && !card.CanPlay(out reason, out _))
                unplayable.Visible = !HasResourceCostReason(reason);
            else
                unplayable.Visible = false;
        }
    }

    public static bool HasResourceCostReason(UnplayableReason reason)
    {
        return reason.HasFlag(UnplayableReason.EnergyCostTooHigh) ||
               reason.HasFlag(UnplayableReason.StarCostTooHigh);
    }

    public static void UpdateBrandColorCostColor(NCard instance, PileType pileType, MegaLabel label,
        HatMagician2Card card)
    {
        Color color1 = StsColors.cream;
        Color color2 = StsColors.defaultStarCostOutline;
        if (!card.HasBrandColorCostX && card.DynamicBrandCost.WasJustUpgraded)
        {
            color1 = StsColors.green;
            color2 = StsColors.energyGreenOutline;
        }
        else if (pileType == PileType.Hand)
        {
            CardCostColor starCostColor = GetStarCostColor(card, card.CombatState);
            var var = typeof(NCard).GetField("_pretendCardCanBePlayed", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(instance);
            bool played = var != null && (bool)var;
            color1 = GetCostTextColorInHand(starCostColor, played, color1);
            color2 = GetCostOutlineColorInHand(starCostColor, played, color2);
        }

        label.AddThemeColorOverride(ThemeConstants.Label.FontColor, color1);
        label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, color2);
    }

    public static CardCostColor GetStarCostColor(HatMagician2Card card, ICombatState? state)
    {
        if (state == null)
            return CardCostColor.Unmodified;
        // UnplayableReason reason;
        // if (!card.CanPlay(out reason, out _) && reason.HasFlag(UnplayableReason.StarCostTooHigh))
        //     return CardCostColor.InsufficientResources;
        if (!card.HasEnoughEnergy())
            return CardCostColor.InsufficientResources;
        if (card.HasBrandColorCostX)
            return CardCostColor.Unmodified;
        Decimal hookModifiedCost;
        if (HatMagician2Mgr.TryModifyBrandColorCostWithHooks(card, state, out hookModifiedCost))
            return GetColorForHookModifiedCost(hookModifiedCost, card.BaseBrandColorCost);
        // 看不懂这行，直接返回无变化
        // return card.TemporaryStarCost != null ? GetColorForLocalCost(card.TemporaryStarCost.Cost, card.BaseBrandColorCost) : CardCostColor.Unmodified;
        return CardCostColor.Unmodified;
    }

    public static CardCostColor GetColorForHookModifiedCost(Decimal hookModifiedCost, int baseCost)
    {
        if (hookModifiedCost > baseCost)
            return CardCostColor.Increased;
        return hookModifiedCost < baseCost ? CardCostColor.Decreased : CardCostColor.Unmodified;
    }

    // private static CardCostColor GetColorForLocalCost(int localCost, int baseCost)
    // {
    //     if (localCost > baseCost)
    //         return CardCostColor.Increased;
    //     return localCost < baseCost ? CardCostColor.Decreased : CardCostColor.Unmodified;
    // }

    public static Color GetCostTextColorInHand(
        CardCostColor costColor,
        bool pretendCardCanBePlayed,
        Color defaultColor)
    {
        // NCard.GetCostTextColorInHand(costColor, pretendCardCanBePlayed, defaultColor);
        switch (costColor)
        {
            case CardCostColor.Unmodified:
                return defaultColor;
            case CardCostColor.Increased:
                return StsColors.energyBlue;
            case CardCostColor.Decreased:
                return StsColors.green;
            case CardCostColor.InsufficientResources:
                return pretendCardCanBePlayed ? defaultColor : StsColors.red;
            default:
                throw new ArgumentOutOfRangeException(nameof(costColor), costColor, null);
        }
    }

    public static Color GetCostOutlineColorInHand(
        CardCostColor costColor,
        bool pretendCardCanBePlayed,
        Color defaultColor)
    {
        // NCard.GetCostOutlineColorInHand(costColor, pretendCardCanBePlayed, defaultColor);
        switch (costColor)
        {
            case CardCostColor.Unmodified:
                return defaultColor;
            case CardCostColor.Increased:
                return StsColors.energyBlueOutline;
            case CardCostColor.Decreased:
                return StsColors.energyGreenOutline;
            case CardCostColor.InsufficientResources:
                return pretendCardCanBePlayed ? defaultColor : StsColors.unplayableEnergyCostOutline;
            default:
                throw new ArgumentOutOfRangeException(nameof(costColor), costColor, null);
        }
    }
}