using BaseLib.Abstracts;
using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Extensions;
using HatMagician2.HatMagician2Code.Character;

namespace HatMagician2.HatMagician2Code.Powers;

public abstract class HatMagician2Power : CustomPowerModel, IHatMagician2AbstractModel
{
    //Loads from TestMTS2Char/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}