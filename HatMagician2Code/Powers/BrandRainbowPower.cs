using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandRainbowPower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.Rainbow;
    protected override decimal BasePassiveVal => 0;
    protected override decimal BaseEvokeVal => 1;
    protected override decimal BaseEvokeVal2 => 3;
    protected override decimal BaseFusionVal => 1;
    protected override decimal BaseFusionVal2 => 1;

    protected override async Task OnFusion(CardModel? cardSource, Creature? oldApplier = null)
    {
        if (this.IsOnFusionEd) return;
        await base.OnFusion(cardSource, oldApplier);
        if ((cardSource?.Owner ?? this.Applier?.Player) is { } player)
            await PlayerCmd.GainEnergy(this.FusionVal, player);
        if (this.Owner.Side == CombatSide.Player)
        {
            var list = this.Owner.Powers.Where(p => p is { TypeForCurrentAmount: PowerType.Debuff, IsVisible: true } and not BrandRainbowPower).ToList();
            foreach (var debuff in list)
            {
                await PowerCmd.Remove(debuff);
            }
        }
    }

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (this.Applier?.Player == null) return;
        await base.OnEvoke(card);
        await PlayerCmd.GainEnergy(this.EvokeVal, card?.Owner ?? this.Applier.Player);
        await PowerCmd.Apply<CollectLightPower>(new ThrowingPlayerChoiceContext(), card?.Owner.Creature ?? this.Applier, this.EvokeVal2, card?.Owner.Creature ?? this.Applier, null);
    }
}