using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Extensions;
using Godot;

namespace HatMagician2.HatMagician2Code.Character;

public class HatMagician2PotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => HatMagician2.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}