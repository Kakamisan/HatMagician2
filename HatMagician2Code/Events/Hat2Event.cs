using BaseLib.Abstracts;
using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Events;

public abstract class Hat2Event : CustomEventModel
{
    public override string CustomInitialPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EventImagePath();
}