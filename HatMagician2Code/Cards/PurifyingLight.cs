using BaseLib.Cards.Variables;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class PurifyingLight() : HatMagician2Card(0, CardType.Skill, CardRarity.Rare, TargetType.None), IHatMagician2AbstractModel
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

        // var tmpBlockVar = new BlockVar(this.DynamicVars.Block.BaseValue, ValueProp.Move);
        // tmpBlockVar.UpdateCardPreview(play.Card, CardPreviewMode.Normal, this.Owner.Creature, true);
        var myAmount = this.DynamicVars.Block.PreviewValue;
        var allyAmount = myAmount / 2;

        foreach (var power in powers)
        {
            await this.CleanPower(power);
            await CreatureCmd.GainBlock(this.Owner.Creature, myAmount, ValueProp.Unpowered, play, true);
            foreach (var ally in enumerable)
                await CreatureCmd.GainBlock(ally, allyAmount, ValueProp.Unpowered, play, true);
        }

        this.CleanSomeAffliction(this.Owner);
        foreach (var ally in enumerable)
            this.CleanSomeAffliction(ally.Player);

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
                list.AddRange(creature.Powers.Where(this.IsCanClean));
            }

            return list;
        }

        return [];
    }

    private async Task CleanPower(PowerModel power)
    {
        if (power is SurroundedPower)
            Hat2CardStateSingleton.SetIsCleanSurroundedPower();
        else
            await PowerCmd.Remove(power);
    }

    private bool IsCanClean(PowerModel power)
    {
        if (this.IsCleanSurroundedPower(power))
            return false;
        return power is { TypeForCurrentAmount: PowerType.Debuff, IsVisible: true };
    }

    // 帝王蟹patch 设置为true当作清除帝王蟹debuff
    private bool IsCleanSurroundedPower(PowerModel power) => power is SurroundedPower && Hat2CardStateSingleton.GetIsCleanSurroundedPower();

    public decimal ModifyVanillaPowerHack(PowerModel power, decimal originValue)
    {
        return this.IsCleanSurroundedPower(power) ? 1M : originValue;
    }

    // 部分卡面上的负面效果要清除
    private void CleanSomeAffliction(Player? player)
    {
        if (player?.PlayerCombatState?.AllCards == null || !player.PlayerCombatState.AllCards.Any()) return;
        foreach (var card in player.PlayerCombatState.AllCards)
        {
            if (card.Affliction is Bound)
            {
                CardCmd.ClearAffliction(card);
                // Log.Info("[   Hat2   ]CleanSomeAffliction");
            }
        }
    }
}