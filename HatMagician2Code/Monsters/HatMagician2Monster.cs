using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using HatMagician2.HatMagician2Code.Extensions;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code.Monsters;

public abstract class HatMagician2Monster : CustomMonsterModel
{
    public override int MaxInitialHp => this.MinInitialHp;

    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/" + this.VanillaScene + ".tscn");

    // 不写这个会报错 先随便用一个凑数
    public override string? CustomVisualPath => "brand_color_blue.tscn".ScenePath();

    // 用官方怪物形象
    protected virtual string VanillaScene => "";
}