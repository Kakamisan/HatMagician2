using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public class Grind() : HatMagician2Card(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override bool IsTest => true;

    protected override IEnumerable<IHoverTip> Hat2ExtraHoverTips => [HoverTipFactory.FromKeyword(HatMagician2Keywords.Color)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.Owner.Creature.Player == null)
            return;
        BrandColor[] list = [BrandColor.Red, BrandColor.Blue, BrandColor.Yellow];
        if (this.IsUpgraded)
        {
            foreach (var color in list)
            {
                await PaletteBottle.AddEnergy(this.Owner.Creature.Player, 1, color);
            }
        }
        else
        {
            var list2 = list.TakeRandom(2, this.Owner.RunState.Rng.CombatEnergyCosts);
            foreach (var color in list2)
            {
                await PaletteBottle.AddEnergy(this.Owner.Creature.Player, 1, color);
            }
        }
    }

    // protected override void OnUpgrade() => base.OnUpgrade();
}