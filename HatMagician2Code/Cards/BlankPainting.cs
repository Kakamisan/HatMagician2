using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(QuestCardPool))]
public class BlankPainting() : HatMagician2Card(-1, CardType.Quest, CardRarity.Quest, TargetType.None)
{
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];

    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Unplayable)];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Unplayable];

    public override int MaxUpgradeLevel => 0;

    public override IReadOnlySet<RoomType> ModifyUnknownMapPointRoomTypes(IReadOnlySet<RoomType> roomTypes)
    {
        if (this.Act2Event || this.Act3Event)
            return new HashSet<RoomType> { RoomType.Event };
        return roomTypes;
    }

    public override EventModel ModifyNextEvent(EventModel currentEvent)
    {
        return this.Act2Event ? ModelDb.Event<DrawProfessorEvent>() : this.Act3Event ? ModelDb.Event<HomelessPersonEvent>() : currentEvent;
    }

    private bool Act2Event
    {
        get
        {
            var runState = RunManager.Instance.DebugOnlyGetState();
            return runState is
            {
                CurrentActIndex: 1, TotalFloor: >= Hat2ModConfigUtil.DrawProfessorEventFloor
            } && !runState.VisitedEventIds.Contains(ModelDb.Event<DrawProfessorEvent>().Id);
        }
    }

    private bool Act3Event => this.Owner.RunState.CurrentActIndex == 2;
}