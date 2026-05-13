using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code.SceneControl;

public static class NCreatureUtil
{
    public static T? InitNode<T>(string scenePath) where T : NCreatureVisuals
    {
        var scene = GD.Load<PackedScene>(scenePath);
        return scene.Instantiate() as T;
    }
}