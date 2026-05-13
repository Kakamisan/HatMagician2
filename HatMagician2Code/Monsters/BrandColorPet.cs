using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code.Monsters;

public partial class BrandColorPet : HatMagician2Pet
{
    // 绘色的类型
    public virtual BrandColor Color => BrandColor.None;
    
    // 场景路径
    public virtual string ScenePath => "";

    public override string CustomVisualPath => this.ScenePath;

    public static T? Create<T>() where T : BrandColorPet
    {
        return null;
    }
}