using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code.Monsters;

public abstract class HatMagician2Monster : CustomMonsterModel
{
    public override int MaxInitialHp => this.MinInitialHp;

    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/" + this.VanillaScene + ".tscn");

    // 用官方怪物形象
    protected virtual string VanillaScene => "";
}