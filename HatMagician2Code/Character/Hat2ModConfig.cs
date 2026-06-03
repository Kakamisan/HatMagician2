using BaseLib.Config;

namespace HatMagician2.HatMagician2Code.Character;

[ConfigHoverTipsByDefault]
public class Hat2ModConfig : SimpleModConfig
{
    public static bool ShowBaseBrandColorTips { get; set; } = true;
    public static bool ShowFusionBrandColorTips { get; set; } = true;
}