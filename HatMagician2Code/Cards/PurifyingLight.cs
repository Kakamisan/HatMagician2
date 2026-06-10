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
public class PurifyingLight() : HatMagician2Card(0, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    public override int BaseBrandColorCost => 1;
    public override bool HasBrandApplyTarget => false;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars =>
    [
        new("PurifyingLightBase", 0), new("PurifyingLightExtra", 1), new BlockVar(5, ValueProp.Move),
        new CustomCalculatedVar("PurifyingLight").WithMultiplier((card, _) => ((PurifyingLight)card).GetPowers().Count)
    ];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var powers = this.GetPowers();

        // (联机)群体加格挡
        var allies = this.CombatState?.PlayerCreatures.Where(c => c is { IsAlive: true, IsPlayer: true } && c != this.Owner.Creature) ?? [];
        var enumerable = allies as Creature[] ?? allies.ToArray();

        decimal allyAmount;
        if (enumerable.Length != 0)
        {
            var tmpBlockVar = new BlockVar(this.DynamicVars.Block.BaseValue, ValueProp.Move);
            tmpBlockVar.UpdateCardPreview(play.Card, CardPreviewMode.Normal, this.Owner.Creature, true);
            allyAmount = tmpBlockVar.PreviewValue / 2;
        }
        else
        {
            allyAmount = 0;
        }

        foreach (var power in powers)
        {
            await PowerCmd.Remove(power);
            await this.CommonBlock(play);
            foreach (var ally in enumerable)
                await CreatureCmd.GainBlock(ally, allyAmount, ValueProp.Unpowered, play, true);
        }

        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(2);

    private List<PowerModel> GetPowers()
    {
        if (this.CombatState != null)
        {
            List<PowerModel> list = [];
            foreach (var creature in this.CombatState.Creatures)
            {
                list.AddRange(creature.Powers.Where(p => p is { TypeForCurrentAmount: PowerType.Debuff, IsVisible: true }));
            }

            return list;
        }

        return [];
    }
}