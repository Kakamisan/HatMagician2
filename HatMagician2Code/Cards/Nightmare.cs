using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Nightmare() : HatMagician2Card(0, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.Orange;
    public override int BaseBrandColorCost => 0;

    public override bool HasBrandApplyTarget => true;

    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new PowerVar<VulnerablePower>(1)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.OnlyDream, HatMagician2Keywords.Dream, CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        foreach (Creature enemy in this.CombatState.HittableEnemies)
        {
            await BrandPower.ApplyBrandPower(this, choiceContext, enemy, this.BaseBrandColor);
        }

        await this.CommonAoeApplyTargetPower<VulnerablePower>(choiceContext, this.DynamicVars.Vulnerable.IntValue);

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.RemoveKeyword(CardKeyword.Exhaust);
}