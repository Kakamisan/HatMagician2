using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using HarmonyLib;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Monsters;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(StatusCardPool))]
public class ColorStatus() : HatMagician2Card(0, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override BrandColor BaseBrandColor => this.DynamicColor;
    public override int BaseBrandColorCost => -1;
    public override bool HasBrandApply => true;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new("Branch", 0)];

    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    public BrandColor DynamicColor = BrandColor.None;
    private const int DynamicCost = 2;

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // var target = this.CombatState?.Enemies.FirstOrDefault(e => e.Monster is ColorFinderPainting);
        // if (target == null) return;
        // await BrandPower.ApplyBrandPower(this, choiceContext, target, this.DynamicColor);
        await this.OnPlayNormal(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = this.CombatState?.Enemies.FirstOrDefault(e => e.Monster is ColorFinderPainting);
        if (target == null) return;
        if (this.CombatState == null) return;
        BrandColor[] list = [BrandColor.Red, BrandColor.Blue, BrandColor.Yellow];
        var list2 = list.Where(color => HatMagician2Mgr.HasEnoughEnergy(this.Owner, color, DynamicCost)).ToList();
        if (list2.Count == 0) return;
        List<ColorStatus> cards = [];
        var card = this.CombatState.CreateCard<ColorStatus>(this.Owner);
        card.DynamicBrandCost.BaseValue = DynamicCost;
        foreach (var color in list2)
        {
            card.DynamicColor = color;
            card.GetDynamicVar("Branch").BaseValue = card.GetColor();
            var card2 = (ColorStatus)card.MutableClone();
            cards.Add(card2);
        }

        var choose = (ColorStatus?)await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, this.Owner, true);
        if (choose == null) return;
        var chooseColor = choose.DynamicColor;
        await HatMagician2Mgr.SpendEnergy(this.Owner, this.BrandColorCost, chooseColor);
        await BrandPower.ApplyBrandPower(this, choiceContext, target, chooseColor);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => base.OnUpgrade();

    public override int MaxUpgradeLevel => 0;

    private int GetColor()
    {
        return (int)this.DynamicColor;
    }
}