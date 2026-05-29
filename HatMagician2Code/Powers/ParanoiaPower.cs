using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class ParanoiaPower : HatMagician2Power, IHatMagician2AbstractModel
{
    public async Task AfterGloomyDamage(Creature target, decimal damage, Creature? dealer)
    {
        if (dealer == this.Owner)
        {
            this.Flash();
            for (int i = 0; i < this.Amount; i++)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), this.CombatState.HittableEnemies, damage, ValueProp.Unpowered | ValueProp.Unblockable, dealer, null);
            }
        }
    }
}