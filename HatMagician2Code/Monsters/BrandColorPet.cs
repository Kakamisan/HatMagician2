using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace HatMagician2.HatMagician2Code.Monsters;

public class BrandColorPet : HatMagician2Pet
{
    // 绘色的类型
    public virtual BrandColor Color => BrandColor.Red;

    // public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    // {
    //     AnimState animState1 = new AnimState("idle", true);
    //     AnimState state1 = new AnimState("disable");
    //     CreatureAnimator animator = new CreatureAnimator(animState1, controller);
    //     animator.AddAnyState("Idle", animState1);
    //     animator.AddAnyState("DisableTrigger", state1);
    //     return animator;
    // }
}