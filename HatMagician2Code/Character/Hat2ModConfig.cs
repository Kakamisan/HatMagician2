using BaseLib.Config;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Character;

[ConfigHoverTipsByDefault]
public class Hat2ModConfig : SimpleModConfig
{
    public static bool ShowBaseBrandColorTips { get; set; } = true;

    public static bool ShowFusionBrandColorTips { get; set; } = true;

    public static bool ChallengeColorFinder { get; set; } = false;

    [ConfigSlider(1, 8)] public static int ShowOthersBrandPet { get; set; } = 2;
    [ConfigSlider(0, 1, 0.1)] public static double ShowOthersBrandPetAlpha { get; set; } = 0.5f;
}

public static class Hat2ModConfigUtil
{
    public static bool ShouldShowPet()
    {
        return HatMagician2Mgr.Instance?.CountOtherSummonPlayers <= Hat2ModConfig.ShowOthersBrandPet - 1;
    }

    // 空白画作最低出现层数
    public const int BlankPaintingEventFloor = 8;

    // 学院教授最低出现层数
    public const int DrawProfessorEventFloor = 25;

    // 是否出现空白画作相关事件 联机模式下不出现
    public static bool ShouldOpenEvent(RunState runState)
    {
        if (!Hat2ModConfig.ChallengeColorFinder)
            return false;
        if (runState.Players.Count > 1)
            return false;
        return true;
    }
}