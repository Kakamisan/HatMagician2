using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandWhitePower : BrandPower
{
    public override BrandColor BaseBrandColor => BrandColor.White;
    protected override decimal BasePassiveVal => 2;
    protected override decimal BaseEvokeVal => 3;
    protected override decimal BaseFusionVal => 7;
    protected override decimal BaseFusionVal2 => 1;

    // protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new BlockVar(5, ValueProp.Unpowered)); // 格挡数

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (!this.Owner.IsAlive) return;
        if (this.Owner.CombatState == null) return;
        if (this.Applier?.Player == null) return;
        await base.OnEvoke(card);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), this.EvokeVal, card?.Owner ?? this.Applier.Player);
    }

    protected override async Task OnFusion(CardModel? cardSource, Creature? oldApplier = null)
    {
        if (this.IsOnFusionEd) return;
        await base.OnFusion(cardSource, oldApplier);
        var applier = cardSource?.Owner.Creature ?? this.Applier;
        if (applier is not null)
            await CreatureCmd.GainBlock(applier, new BlockVar(this.FusionVal, ValueProp.Unpowered), null);

        // 群体加格挡
        var allies = this.CombatState.PlayerCreatures.Where(c => c is { IsAlive: true, IsPlayer: true } && c != applier);
        foreach (var ally in allies)
            await CreatureCmd.GainBlock(ally, new BlockVar(this.FusionVal / 2, ValueProp.Unpowered), null);
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != this.Owner.Side) return;
        await this.OnPassive();
        await base.AfterSideTurnStart(side, participants, combatState);
    }

    protected override async Task OnPassive(bool setFlag = true)
    {
        if (this.Owner.CombatState == null) return;
        if (this.Applier?.Player == null) return;
        await base.OnPassive(setFlag);
        await UsePassive(this);
    }

    public static async Task UsePassive(BrandPower power, CardModel? card = null, int cnt = 1)
    {
        for (int i = 0; i < cnt; i++)
        {
            await HatMagician2Mgr.AddEnergy(card?.Owner ?? power.Applier!.Player!, (int)power.PassiveVal);
        }

        await Task.CompletedTask;
    }
}