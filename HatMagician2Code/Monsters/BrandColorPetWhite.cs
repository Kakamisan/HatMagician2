using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Monsters;

public partial class BrandColorPetWhite : BrandColorPet
{
    public override BrandColor Color => BrandColor.White;

    public override string ScenePath => "brand_color_white.tscn".ScenePath();
}