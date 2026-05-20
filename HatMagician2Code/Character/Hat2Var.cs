using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Character;

// 卡牌通用额外变量
public class Hat2Var(decimal value, DynamicVar? cosVar = null) : DynamicVar(DefaultName, value)
{
    public const string DefaultName = "Hat2Var";

    // 模仿的原版Var 可能每个版本都要检查一下原版逻辑有没改动 该死的安东尼不把Reset写成virtual！！！
    public DynamicVar? CosVar => cosVar;

    private ValueProp Props
    {
        get
        {
            if (cosVar is BlockVar var)
                return var.Props;
            return ValueProp.Move;
        }
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        if (CosVar is BlockVar)
        {
            decimal originalBlock1 = this.BaseValue;
            EnchantmentModel? enchantment = card.Enchantment;
            if (enchantment != null)
            {
                decimal originalBlock2 = originalBlock1 + enchantment.EnchantBlockAdditive(originalBlock1, this.Props);
                originalBlock1 = originalBlock2 * enchantment.EnchantBlockMultiplicative(originalBlock2, this.Props);
                if (!card.IsEnchantmentPreview)
                    this.EnchantedValue = originalBlock1;
            }

            if (runGlobalHooks && card.CombatState != null)
                originalBlock1 = Hook.ModifyBlock(card.CombatState, card.Owner.Creature, this.BaseValue, this.Props, card, null, out var _);
            this.PreviewValue = originalBlock1;
        }

        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}