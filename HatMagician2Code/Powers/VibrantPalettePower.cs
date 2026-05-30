using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Powers;

public class VibrantPalettePower : HatMagician2Power, IHatMagician2AbstractModel
{
    private bool _trigger;

    public async Task AfterSpendEnergy(Player player, int amount, BrandColor color)
    {
        if (this._trigger) return;
        this._trigger = true;
        this.Flash();
        await HatMagician2Mgr.AddEnergy(player, this.Amount, color);
        await Task.CompletedTask;
    }

    public override Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        this._trigger = false;
        return base.AfterSideTurnEndLate(choiceContext, side, participants);
    }
}