namespace HatMagician2.HatMagician2Code.Character;

[Flags]
public enum BrandColor
{
    None = 0, // 无 一般卡牌 不属于绘色牌
    Red = 0b001,
    Blue = 0b010,
    Yellow = 0b100,
    Purple = Red | Blue,
    Orange = Red | Yellow,
    White = Blue | Yellow,
    Rainbow = Red | Blue | Yellow, // 虹色 只有印记有此颜色

    Any = 0b1000, // 任意 用于消耗任意绘色*N的牌
    All = 0b10000 // 所有 用于消耗所有类型绘色*N的牌
}