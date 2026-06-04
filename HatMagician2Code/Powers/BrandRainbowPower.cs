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
    protected override decimal BaseFusionVal2 => 1;

    protected override async Task OnFusion(HatMagician2Card? card)
    {
        if (this.IsOnFusionEd) return;
        if (this.Applier?.Player == null) return;
        await base.OnFusion(card);
        await HatMagician2Mgr.AddEnergy(card?.Owner ?? this.Applier.Player, (int)this.FusionVal2, this.BaseBrandColor);
        await PlayerCmd.GainEnergy(this.FusionVal, card?.Owner ?? this.Applier.Player);
    }

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (this.Applier?.Player == null) return;
        await base.OnEvoke(card);
        await PlayerCmd.GainEnergy(this.EvokeVal, card?.Owner ?? this.Applier.Player);
        await PowerCmd.Apply<CollectLightPower>(new ThrowingPlayerChoiceContext(), card?.Owner.Creature ?? this.Applier, this.EvokeVal2, card?.Owner.Creature ?? this.Applier, null);
    }
}