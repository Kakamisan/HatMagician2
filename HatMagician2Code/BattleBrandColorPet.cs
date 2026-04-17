using Godot;
using HatMagician2.HatMagician2Code.Monsters;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HatMagician2.HatMagician2Code;

public partial class BattleBrandColorPet : NCreatureVisuals
{
	private MegaLabel? _brandColorEnergyLabel;

	private int _energy;

	public override void _Ready()
	{
		base._Ready();
		this._brandColorEnergyLabel = this.GetNode<MegaLabel>((NodePath)"%PassiveAmount");
	}

	public void UpdateVisuals()
	{
		if (!this.IsNodeReady() || !CombatManager.Instance.IsInProgress)
			return;
		this._brandColorEnergyLabel?.SetTextAutoSize(this._energy.ToString());
	}

	public void SetEnergy(int energy)
	{
		this._energy = energy;
		this.UpdateVisuals();
	}
}
