using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Events;
using MegaCrit.Sts2.Core.Entities.Ascension;
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

    // 进阶10以上时，第1幕8层之后出现的事件50%概率更改为遭遇空白画作
    public override EventModel ModifyNextEvent(EventModel currentEvent)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        var specEvent = ModelDb.Event<BlankPaintingEvent>();
        if (runState != null && Hat2ModConfigUtil.ShouldOpenEvent(runState)
                             && !runState.VisitedEventIds.Contains(specEvent.Id)
                             && runState is { CurrentActIndex: 0, TotalFloor: >= Hat2ModConfigUtil.BlankPaintingEventFloor }
                             && RunManager.Instance.AscensionManager.HasLevel(AscensionLevel.DoubleBoss)
                             && runState.Rng.UnknownMapPoint.NextFloat() < 0.5f
           )
        {
            return specEvent;
        }

        // Log.Info("[   Hat2   ]ModifyNextEvent");

        return base.ModifyNextEvent(currentEvent);
    }
}