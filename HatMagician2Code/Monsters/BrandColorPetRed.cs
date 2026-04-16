using BaseLib.Utils.NodeFactories;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code.Monsters;

public class BrandColorPetRed : BrandColorPet
{
    public override HatMagician2BrandColor Color => HatMagician2BrandColor.Red;

    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("brand_color_red.tscn".ScenePath());

}