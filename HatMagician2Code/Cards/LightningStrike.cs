using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class LightningStrike() : HatMagician2Card(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override int BaseBrandColorCost => 1;

    public override BrandColor BaseBrandColor => BrandColor.Yellow;
    
    public override bool HasBrandApply => true;

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new DamageVar(8, ValueProp.Move)];

    protected override async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BrandPower.ApplyBrandPower(this, choiceContext, play, this.BaseBrandColor);
        await this.OnPlayNormal(choiceContext, play);
    }
    
    protected override async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await this.CommonSingleAttack(choiceContext, play);
        await base.OnPlayNormal(choiceContext, play);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3M);
}