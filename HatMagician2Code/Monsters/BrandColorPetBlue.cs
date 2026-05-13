using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Monsters;

public partial class BrandColorPetBlue : BrandColorPet
{
    public override BrandColor Color => BrandColor.Blue;

    public override string ScenePath => "brand_color_blue.tscn".ScenePath();
}