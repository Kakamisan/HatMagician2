using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;

namespace HatMagician2.HatMagician2Code.Monsters;

public partial class BrandColorPetYellow : BrandColorPet
{
    public override BrandColor Color => BrandColor.Yellow;

    public override string ScenePath => "brand_color_yellow.tscn".ScenePath();
}