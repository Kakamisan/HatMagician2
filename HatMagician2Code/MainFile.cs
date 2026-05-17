using BaseLib.Config;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Modding;

namespace HatMagician2.HatMagician2Code;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "HatMagician2"; //Used for resource filepath

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
        
        ModConfigRegistry.Register(ModId, new Hat2ModConfig());
        
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(MainFile).Assembly);
    }
}