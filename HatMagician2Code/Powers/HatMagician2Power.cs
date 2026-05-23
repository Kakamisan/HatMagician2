using BaseLib.Abstracts;
using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Extensions;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HatMagician2.HatMagician2Code.Powers;

public abstract class HatMagician2Power : CustomPowerModel, IHatMagician2AbstractModel
{
    //Loads from TestMTS2Char/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public virtual bool HasChangeBrandValEffect => false;
    
    // 在死亡动画前移除
    public override async Task BeforeDeath(Creature creature)
    {
        if (this.Owner == creature && this.Type == PowerType.Buff)
        {
            // 怪物在他的回合死亡时移除这个能力
            await PowerCmd.Remove(this);
        }

        await base.BeforeDeath(creature);
    }
}