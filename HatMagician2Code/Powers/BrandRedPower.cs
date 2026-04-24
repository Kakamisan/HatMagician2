using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandRedPower : BrandPower
{
    public BrandRedPower()
    {
        BaseBrandColor = HatMagician2BrandColor.Red;
        BasePassiveVal = 6;
        BaseEvokeVal = 1;
    }

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        //await PowerCmd.Apply<BrandPower>(new ThrowingPlayerChoiceContext(), this.Owner, this.EvokeVal, card?.Owner.Creature, card);
        await base.OnEvoke(card);
    }
}