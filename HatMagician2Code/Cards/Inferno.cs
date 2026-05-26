using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Inferno() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.Red;
    public override int BaseBrandColorCost => 3;
    public override bool HasBrandApplyTarget => true;
    public override TargetType? SubTargetType => TargetType.None;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromCard<Burn>()];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        foreach (Creature enemy in this.CombatState.HittableEnemies)
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, enemy, this.BaseBrandColor);
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(this.CombatState.CreateCard<Burn>(this.Owner), PileType.Discard, this.Owner));
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicBrandCost.UpgradeValueBy(-1);
}