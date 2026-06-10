using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class OrangePainting : HatMagician2Relic, IHatMagician2AbstractModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new Hat2Var(3)];

    private const int AddChainDamageAmount = 1;

    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == this.Owner.Creature.Side && this.Owner.Creature.CombatState != null)
        {
            if (Owner.Creature.CombatState.HittableEnemies.Any(c => c.HasPower<BrandPower>()))
            {
                this.Flash();
                await BrandPower.ChainDamageCmdFromCard(this.Owner.Creature.CombatState, this.DynamicHat2Var.IntValue, this.Owner.Creature, null);
            }
        }

        await base.AfterSideTurnEndLate(choiceContext, side, participants);
    }

    public bool TryModifyChainDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, out decimal modifiedDamage)
    {
        if (dealer == this.Owner.Creature)
        {
            modifiedDamage = amount + AddChainDamageAmount;
            return true;
        }

        modifiedDamage = amount;
        return false;
    }

    public bool TryModifyEvokeValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandYellowPower && power.Owner.Side != this.Owner.Creature.Side)
        {
            modifiedVal = originVal + AddChainDamageAmount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyPassiveValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandYellowPower or BrandOrangePower && power.Owner.Side != this.Owner.Creature.Side)
        {
            modifiedVal = originVal + AddChainDamageAmount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }

    public bool TryModifyFusionValAdditive(BrandPower power, decimal originVal, out decimal modifiedVal)
    {
        if (power is BrandOrangePower && power.Owner.Side != this.Owner.Creature.Side)
        {
            modifiedVal = originVal + AddChainDamageAmount;
            return true;
        }

        modifiedVal = originVal;
        return false;
    }
}