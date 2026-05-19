using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class InversionOfRealityAndDream() : HatMagician2Card(2, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // public override BrandColor BaseBrandColor => BrandColor.None;
    // public override int BaseBrandColorCost => -1;
    // public override bool HasBrandApply => false;
    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    // protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust, HatMagician2Keywords.Dream];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    private bool _needExtraTurn;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var cards1 = PileType.Draw.GetPile(this.Owner).Cards.ToList();
        var cards2 = PileType.Discard.GetPile(this.Owner).Cards.ToList();
        foreach (var card in cards1)
        {
            await CardCmd.Discard(choiceContext, card);
        }

        if (cards2.Count > 0)
        {
            await CardPileCmd.Add(cards2, PileType.Draw, CardPilePosition.Random);
        }

        this._needExtraTurn = true;
        
        PlayerCmd.EndTurn(this.Owner, false);

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
    
    public override bool ShouldTakeExtraTurn(Player player)
    {
        if (player == this.Owner)
        {
            var flag = this._needExtraTurn;
            this._needExtraTurn = false;
            return flag;
        }

        return false;
    }
}