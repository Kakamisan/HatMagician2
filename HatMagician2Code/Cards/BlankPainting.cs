using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(ColorlessCardPool))]
public class BlankPainting : HatMagician2Card
{
    public BlankPainting() : base(-1, CardType.Quest, CardRarity.Quest, TargetType.None)
    {
        IsTest = true;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Unplayable)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
    }

    protected override void OnUpgrade()
    {
    }
}