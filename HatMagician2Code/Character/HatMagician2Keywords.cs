using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace HatMagician2.HatMagician2Code.Character;

public class HatMagician2Keywords
{
    [CustomEnum("CHAIN")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Chain;    // 连锁
    [CustomEnum("EVOKE")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Evoke;    // 刻印
    [CustomEnum("FUSION")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Fusion;   // 叠色
    [CustomEnum("COLOR")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Color;    // 绘色
    [CustomEnum("BASE_COLOR")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword BaseColor;    // 绘色
    [CustomEnum("SLEEP")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Sleep;    // 睡衣
    [CustomEnum("ONLY_DREAM")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword OnlyDream;// 仅梦乡（无法通过手牌打出）
    [CustomEnum("DREAM")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Dream;    // 梦乡
}