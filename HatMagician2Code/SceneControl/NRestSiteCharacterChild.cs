using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.RestSite;

namespace HatMagician2.HatMagician2Code.SceneControl;

[GlobalClass]
public partial class NRestSiteCharacterChild : NRestSiteCharacter
{
	public override void _Ready()
	{
		this.GetNode<Sprite2D>((NodePath)"overgrowth").Visible = this.Player.RunState.CurrentActIndex == 0;
		this.GetNode<Sprite2D>((NodePath)"hive").Visible = this.Player.RunState.CurrentActIndex == 1;
		this.GetNode<Sprite2D>((NodePath)"glory").Visible = this.Player.RunState.CurrentActIndex == 2;
		Log.Info("[   Rest   ] Index:" + this.Player.RunState.CurrentActIndex);
		base._Ready();
	}
}
