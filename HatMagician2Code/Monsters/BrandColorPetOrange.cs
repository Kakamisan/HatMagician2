using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Monsters;

public partial class BrandColorPetOrange : BrandColorPet
{
    public override BrandColor Color => BrandColor.Orange;

    public override string ScenePath => "brand_color_orange.tscn".ScenePath();
}