using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Incinerate() : HatMagician2Card(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override BrandColor BaseBrandColor => BrandColor.Red;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => this.GetCount() > 0;
    public override bool HasBrandApply => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    public override TargetType? SubTargetType => TargetType.None;

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new CalculationBaseVar(0), new CalculationExtraVar(1),
        new CalculatedVar("CalculatedHits").WithMultiplier((card, _) => GetStatuses(card.Owner).Count())
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var list = GetStatuses(this.Owner).ToList();
        var cnt = ((CalculatedVar)this.GetDynamicVar("CalculatedHits")).Calculate(play.Target!);
        foreach (CardModel card2 in list)
            await CardCmd.Exhaust(choiceContext, card2);
        for (int i = 0; i < cnt; i++)
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.RemoveKeyword(CardKeyword.Exhaust);

    private static IEnumerable<CardModel> GetStatuses(Player owner)
    {
        if (owner.PlayerCombatState == null) return [];
        return owner.PlayerCombatState.AllCards.Where(c => c is { Type: CardType.Status, Pile: not null } && c.Pile.Type != PileType.Exhaust);
    }

    private int GetCount()
    {
        return this.CombatState != null ? GetStatuses(this.Owner).Count() : 0;
    }
}