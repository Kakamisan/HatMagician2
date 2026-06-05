using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Monsters;

public abstract class HatMagician2Pet(bool visibleHp) : CustomPetModel(visibleHp)
{
    public override int MinInitialHp => 9999;
    public override int MaxInitialHp => 9999;
    
    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    //public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("brand_color_red.tscn".ScenePath());
    
    // 如果你挂载了自己的自定义脚本，使用这个
    // public override string? CustomVisualPath => "brand_color_orange.tscn".ScenePath();

    public HatMagician2Pet() : this(false)
    {
    }
}