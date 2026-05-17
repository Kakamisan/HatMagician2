using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class DessertTime() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.White;

    public override int BaseBrandColorCost => 1;

    // protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new HealVar(3), new Hat2Var(3)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    // protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        var value1 = this.DynamicVars.Heal.IntValue + this.DynamicHat2Var.IntValue;
        var value2 = this.DynamicVars.Heal.IntValue;
        var list1 = this.CombatState.GetTeammatesOf(this.Owner.Creature).Where(c => c.IsAlive);
        var list2 = this.CombatState.Enemies.Where(c => c.IsAlive);
        foreach (var creature in list1)
        {
            await CreatureCmd.Heal(creature, value1);
        }

        foreach (var creature in list2)
        {
            await CreatureCmd.Heal(creature, value2);
        }

        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        var value = this.DynamicVars.Heal.IntValue;
        var list = this.CombatState.Creatures.Where(c => c.IsAlive);
        foreach (var creature in list)
        {
            await CreatureCmd.Heal(creature, value);
        }

        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Heal.UpgradeValueBy(2);
        this.DynamicHat2Var.UpgradeValueBy(1);
    }
}