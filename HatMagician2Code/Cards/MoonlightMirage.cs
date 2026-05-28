using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class MoonlightMirage() : HatMagician2Card(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    public override int BaseBrandColorCost => 2;
    public override bool HasBrandApplyTarget => false;
    public override TargetType? SubTargetType => TargetType.AnyEnemy;
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];
    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(16, ValueProp.Move), new BlockVar(8, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> Hat2CanonicalKeywords => [HatMagician2Keywords.Sleep];
    protected override HashSet<CardTag> Hat2CanonicalTags => [];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        var cnt = this.CombatState.HittableEnemies.Count;
        await this.CommonAoeAttack(choiceContext, play);
        for (int i = 0; i < cnt + 1; i++)
        {
            await this.CommonBlock(play);
        }

        await base.OnPlayWhenCostBrandColor(choiceContext, play);
    }

    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonSingleAttack(choiceContext, play);
        await this.CommonBlock(play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(4);
        this.DynamicVars.Block.UpgradeValueBy(2);
    }
}