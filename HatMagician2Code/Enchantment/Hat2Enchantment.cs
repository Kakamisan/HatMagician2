using BaseLib.Abstracts;
using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Enchantment;

public class Hat2Enchantment : CustomEnchantmentModel
{
    protected override string CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentPath();

    // 是否在卡牌上显示数值
    public override bool ShowAmount => true;

    // 重载这个以改变显示的数字
    // public override int DisplayAmount => DynamicVars.Cards.IntValue;

    // 是否会添加额外的卡牌描述文本
    public override bool HasExtraCardText => true;

    public virtual bool IsEvoke => false;
}