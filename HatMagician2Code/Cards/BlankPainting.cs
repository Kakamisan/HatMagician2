using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(QuestCardPool))]
public class BlankPainting() : HatMagician2Card(-1, CardType.Quest, CardRarity.Quest, TargetType.None)
{
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];

    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Unplayable)];

    public override int MaxUpgradeLevel => 0;

    protected override void OnUpgrade()
    {
    }
}