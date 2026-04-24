namespace HatMagician2.HatMagician2Code.Character;

public enum HatMagician2BrandColor
{
    Red,
    Blue,
    Yellow,
    Purple,
    Orange,
    White,
    Rainbow,    // 虹色 只有印记有此颜色
    
    Any,        // 任意 用于消耗任意绘色*N的牌
    All,        // 所有 用于消耗所有类型绘色*N的牌
    None        // 无 一般卡牌 不属于绘色牌
}