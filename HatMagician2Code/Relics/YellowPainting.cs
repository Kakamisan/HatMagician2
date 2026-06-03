using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Enchantment;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace HatMagician2.HatMagician2Code.Relics;

[Pool(typeof(HatMagician2RelicPool))]
public class YellowPainting : HatMagician2Relic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [..HoverTipFactory.FromEnchantment<LightningEnchantment>()];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        this.Flash();
        var list = PileType.Draw.GetPile(this.Owner).Cards.Where(c => c is HatMagician2Card { TargetType: TargetType.AnyEnemy, Enchantment: null }).ToList()
            .StableShuffle(this.Owner.RunState.Rng.CombatCardSelection).Take(this.DynamicVars.Cards.IntValue).ToList();
        foreach (var card in list)
            CardCmd.Enchant<LightningEnchantment>(card, 1);
        CardCmd.Preview(list);
        await Cmd.CustomScaledWait(0.5f, 1f);
    }
}