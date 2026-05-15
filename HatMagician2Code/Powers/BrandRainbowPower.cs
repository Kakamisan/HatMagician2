using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandRainbowPower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.Rainbow;
    protected override decimal BasePassiveVal => 0;
    protected override decimal BaseEvokeVal => 0;
    protected override decimal BaseFusionVal => 0;

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        await base.OnFusion(cardSource);
        if (this.Applier?.Player == null)
            return;
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, 1, this.BaseBrandColor);
        await PlayerCmd.GainEnergy(this.FusionVal, this.Applier.Player);
    }
}