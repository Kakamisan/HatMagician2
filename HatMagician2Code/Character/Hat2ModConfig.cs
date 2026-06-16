using BaseLib.Config;

namespace HatMagician2.HatMagician2Code.Character;

[ConfigHoverTipsByDefault]
public class Hat2ModConfig : SimpleModConfig
{
    public static bool ShowBaseBrandColorTips { get; set; } = true;
    public static bool ShowFusionBrandColorTips { get; set; } = true;
    public static bool ChallengeColorFinder { get; set; } = true;
    [ConfigSlider(1, 8)] 
    public static int ShowOthersBrandPet { get; set; } = 2;
    [ConfigSlider(0, 1, 0.1)] 
    public static double ShowOthersBrandPetAlpha { get; set; } = 0.5f;
}

public static class Hat2ModConfigUtil
{
    public static bool ShouldShowPet()
    {
        return HatMagician2Mgr.Instance?.CountOtherSummonPlayers <= Hat2ModConfig.ShowOthersBrandPet - 1;
    }
}