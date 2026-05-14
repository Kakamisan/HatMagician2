using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Character;

// 卡面记述的绘色消耗数值
public class BrandColorCostVar(decimal cost) : DynamicVar(DefaultName, cost)
{
    public const string DefaultName = "BrandColorCostVar";
}