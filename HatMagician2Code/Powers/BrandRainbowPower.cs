using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandRainbowPower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.Rainbow;
    protected override decimal BasePassiveVal => 0;
    protected override decimal BaseEvokeVal => 1;
    protected override decimal BaseEvokeVal2 => 3;
    protected override decimal BaseFusionVal => 1;

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        if (this.IsOnFusionEd) return;
        if (this.Applier?.Player == null) return;
        await base.OnFusion(cardSource);
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, 1, this.BaseBrandColor);
        await PlayerCmd.GainEnergy(this.FusionVal, this.Applier.Player);
    }

    protected override async Task OnEvoke(HatMagician2Card? cardSource)
    {
        if (this.Applier?.Player == null) return;
        await base.OnEvoke(cardSource);
        await PlayerCmd.GainEnergy(this.EvokeVal, this.Applier.Player);
        await PowerCmd.Apply<CollectLightPower>(new ThrowingPlayerChoiceContext(), this.Applier, this.EvokeVal2, this.Applier, null);
    }
}