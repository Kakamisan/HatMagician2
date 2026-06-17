using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

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
        // todo 第二幕和第三幕的事件
        return this.Act2Event ? ModelDb.Event<WarHistorianRepy>() : this.Act3Event ? ModelDb.Event<WarHistorianRepy>() : currentEvent;
    }

    // 第二幕触发一个事件 第三幕进入画界并触发第二个事件
    [SavedProperty] public int NextActIndex = 1;

    private bool Act2Event => this.NextActIndex == 1 && this.Owner.RunState.CurrentActIndex == 1;

    private bool Act3Event => this.NextActIndex == 2 && this.Owner.RunState.CurrentActIndex == 2;
}