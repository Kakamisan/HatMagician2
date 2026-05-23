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

    // 设置cos原版Var 功能等于原版
    // 没找到在哪deep clone 所以用的时候每次都new一个新的
    public DynamicVar? CosVar { get; set; } = cosVar;

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        if (CosVar != null)
        {
            CosVar.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
            this.PreviewValue = CosVar.PreviewValue;
            this.EnchantedValue = CosVar.EnchantedValue;
            CosVar.ResetToBase();
        }

        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}