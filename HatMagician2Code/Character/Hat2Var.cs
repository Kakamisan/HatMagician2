using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Character;

// 卡牌通用额外变量
public class Hat2Var(decimal value) : DynamicVar(DefaultName, value)
{
    public const string DefaultName = "Hat2Var";
}