using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class PurifyingLight() : HatMagician2Card(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new("PurifyingLightBase", 0), new("PurifyingLightExtra", 4),
        new CustomCalculatedBlockVar("PurifyingLight", ValueProp.Unpowered).WithMultiplier((card, _) => ((PurifyingLight)card).GetPowers().Count)
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [CardKeyword.Exhaust];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var powers = this.GetPowers();
        foreach (var power in powers)
        {
            await PowerCmd.Remove(power);
            await CreatureCmd.GainBlock(this.Owner.Creature, this.GetDynamicVar("PurifyingLightExtra").IntValue, ValueProp.Unpowered, play);
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.GetDynamicVar("PurifyingLightExtra").UpgradeValueBy(2);

    private List<PowerModel> GetPowers()
    {
        if (this.CombatState != null)
        {
            List<PowerModel> list = [];
            foreach (var creature in this.CombatState.Creatures)
            {
                list.AddRange(creature.Powers.Where(p => p.Type == PowerType.Debuff || p is HatMagician2Power { FakeDebuff: true }));
            }

            return list;
        }

        return [];
    }
}