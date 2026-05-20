using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Grind() : HatMagician2Card(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(HatMagician2Keywords.Color), HoverTipFactory.FromKeyword(HatMagician2Keywords.BaseColor)];

    protected override IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [new Hat2Var(2), new RepeatVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.Owner.Creature.Player == null)
            return;
        if (HatMagician2Mgr.Instance == null)
            return;
        List<BrandColor> list = [BrandColor.Red, BrandColor.Yellow, BrandColor.Blue];
        for (int i = 0; i < this.DynamicVars.Repeat.IntValue; i++)
        {
            var color = list.OrderBy(c => HatMagician2Mgr.Instance.GetState(this.Owner.Creature.Player).GetEnergy(c)).First();
            await HatMagician2Mgr.AddEnergy(this.Owner.Creature.Player, this.DynamicHat2Var.IntValue, color);
        }
    }

    protected override void OnUpgrade() => this.DynamicVars.Repeat.UpgradeValueBy(1);
}