using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Character;

public class SortStateVar() : DynamicVar(DefaultName, 1)
{
    public const string DefaultName = "SortState";

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        // if (HatMagician2Mgr.Instance == null) return;
        // List<BrandColor> list = [BrandColor.Red, BrandColor.Yellow, BrandColor.Blue];
        // var colors = list.OrderByDescending(c => HatMagician2Mgr.Instance.GetState(card.Owner).GetEnergy(c)).ToList();
        // this.BaseValue = colors switch
        // {
        //     [BrandColor.Red, BrandColor.Yellow, BrandColor.Blue] => 1,
        //     [BrandColor.Red, BrandColor.Blue, BrandColor.Yellow] => 2,
        //     [BrandColor.Yellow, BrandColor.Red, BrandColor.Blue] => 3,
        //     [BrandColor.Yellow, BrandColor.Blue, BrandColor.Red] => 4,
        //     [BrandColor.Blue, BrandColor.Red, BrandColor.Yellow] => 5,
        //     _ => 6
        // };
        // this.ResetToBase();
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }

    public List<BrandColor> Value2Colors()
    {
        List<BrandColor> colors = this.BaseValue switch
        {
            1 => [BrandColor.Red, BrandColor.Yellow, BrandColor.Blue],
            2 => [BrandColor.Red, BrandColor.Blue, BrandColor.Yellow],
            3 => [BrandColor.Yellow, BrandColor.Red, BrandColor.Blue],
            4 => [BrandColor.Yellow, BrandColor.Blue, BrandColor.Red],
            5 => [BrandColor.Blue, BrandColor.Red, BrandColor.Yellow],
            _ => [BrandColor.Blue, BrandColor.Yellow, BrandColor.Red]
        };
        return colors;
    }
}