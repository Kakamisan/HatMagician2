using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class RainbowPainting : HatMagician2Relic
{
    private int _hatMagician2BrandCardPlayed;
    private bool _isActivating;

    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool ShowCounter => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(7), new EnergyVar(1)];

    public override int DisplayAmount => !this.IsActivating ? this.HatMagician2BrandCardPlayed % this.DynamicVars.Cards.IntValue : this.DynamicVars.Cards.IntValue;

    [SavedProperty]
    public int HatMagician2BrandCardPlayed
    {
        get => this._hatMagician2BrandCardPlayed;
        set
        {
            this.AssertMutable();
            this._hatMagician2BrandCardPlayed = value;
            this.UpdateDisplay();
        }
    }

    private bool IsActivating
    {
        get => this._isActivating;
        set
        {
            this.AssertMutable();
            this._isActivating = value;
            this.UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (this.IsActivating)
        {
            this.Status = RelicStatus.Normal;
        }
        else
        {
            int intValue = this.DynamicVars.Cards.IntValue;
            this.Status = this.HatMagician2BrandCardPlayed % intValue == intValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        this.InvokeDisplayAmountChanged();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner)
            return;
        if (cardPlay.Card is not HatMagician2Card { HasBrandApply: true })
            return;
        this.HatMagician2BrandCardPlayed++;
        int intValue = this.DynamicVars.Cards.IntValue;
        if (!CombatManager.Instance.IsInProgress || this.HatMagician2BrandCardPlayed % intValue != 0)
            return;
        _ = TaskHelper.RunSafely(this.DoActivateVisuals());
        await HatMagician2Mgr.AddEnergy(this.Owner, this.DynamicVars.Energy.IntValue, BrandColor.Rainbow);
        await base.AfterCardPlayed(choiceContext, cardPlay);
    }

    private async Task DoActivateVisuals()
    {
        this.IsActivating = true;
        this.Flash();
        await Cmd.Wait(1f);
        this.IsActivating = false;
    }
}