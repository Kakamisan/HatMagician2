using BaseLib.Audio;

namespace HatMagician2.HatMagician2Code.Character;

public static class Hat2Audio
{
    public static readonly ModSound ColorFinderBgm = new(ColorFinderBgmPath, ModAudio.SoundType.Music);
    public static string ColorFinderBgmPath => $"{MainFile.ResPath}/music/first_dance_clip.mp3";
}