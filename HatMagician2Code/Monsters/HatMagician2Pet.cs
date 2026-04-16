using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Character;

namespace HatMagician2.HatMagician2Code.Monsters;

public class HatMagician2Pet(bool visibleHp) : CustomPetModel(visibleHp)
{
    public override int MinInitialHp => 9999;
    public override int MaxInitialHp => 9999;

    public HatMagician2Pet() : this(false)
    {
    }
}