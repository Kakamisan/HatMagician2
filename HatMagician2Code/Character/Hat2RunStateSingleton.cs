using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Character;

public class Hat2RunStateSingleton : CustomSingletonModel
{
    public Hat2RunStateSingleton() : base(HookType.Run)
    {
        _instance = this;
    }

    private static Hat2RunStateSingleton? _instance;

    // 8层之后出现的事件必定遭遇此事件
    public override EventModel ModifyNextEvent(EventModel currentEvent)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        var specEvent = ModelDb.Event<BlankPaintingEvent>();
        if (runState != null && !runState.VisitedEventIds.Contains(specEvent.Id) && runState is { CurrentActIndex: 0, TotalFloor: >= Hat2ModConfigUtil.BlankPaintingEventFloor })
        {
            return specEvent;
        }

        // Log.Info("[   Hat2   ]ModifyNextEvent");

        return base.ModifyNextEvent(currentEvent);
    }
}