using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Monsters;

public partial class BrandColorPetRed : BrandColorPet
{
    public override BrandColor Color => BrandColor.Red;

    public override string ScenePath => "brand_color_red.tscn".ScenePath();
}