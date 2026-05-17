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
    protected override decimal BasePassiveVal => 1;
    protected override decimal BaseEvokeVal => 2;
    protected override decimal BaseFusionVal => 6;

    // protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Append(new BlockVar(5, ValueProp.Unpowered)); // 格挡数

    protected override async Task OnEvoke(HatMagician2Card? card)
    {
        if (!this.Owner.IsAlive) return;
        if (this.Owner.CombatState == null) return;
        if (this.Applier?.Player == null) return;
        await base.OnEvoke(card);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), this.EvokeVal, this.Applier.Player);
    }

    protected override async Task OnFusion(HatMagician2Card? cardSource)
    {
        if (this.Applier?.Player == null) return;
        await base.OnFusion(cardSource);
        await HatMagician2Mgr.AddEnergy(this.Applier.Player, 1, this.BaseBrandColor);
        await CreatureCmd.GainBlock(this.Applier, new BlockVar(this.FusionVal, ValueProp.Unpowered), null);
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != this.Owner.Side) return;
        await this.OnPassive();
        await base.AfterSideTurnStart(side, combatState);
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
            await HatMagician2Mgr.AddEnergy(card != null ? card.Owner : power.Applier!.Player!, (int)power.PassiveVal);
        }

        await Task.CompletedTask;
    }
}