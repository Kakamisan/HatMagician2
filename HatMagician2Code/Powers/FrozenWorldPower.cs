using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HatMagician2.HatMagician2Code.Powers;

public class FrozenWorldPower : HatMagician2Power
{
    public override PowerStackType StackType => this.Amount > 1 ? PowerStackType.Counter : PowerStackType.Single;

    // 弃牌阶段后触发（抽牌保留到下回合）
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        // 这个是弃牌阶段前触发（抽牌不保留到下回合）
        // public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == this.Owner.Side)
        {
            var enemies = this.CombatState.HittableEnemies.ToList();
            this.Flash();
            for (int i = 0; i < this.Amount; i++)
            {
                foreach (var e in enemies)
                {
                    await BrandPower.ApplyBrandPower(null, this.Owner, new ThrowingPlayerChoiceContext(), e, BrandColor.Blue);
                }
            }
        }

        await base.AfterSideTurnEnd(choiceContext, side, participants);
        // await base.BeforeSideTurnEndEarly(choiceContext, side, participants);
    }
}