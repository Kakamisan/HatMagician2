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
    private Node2D? _body;
    private bool _isTween;

    public override void _Ready()
    {
        base._Ready();
        this._brandColorEnergyLabel = this.GetNode<MegaLabel>((NodePath)"%PassiveAmount");
        this._body = this.GetNode<Node2D>((NodePath)"%Visuals");
    }

    public void UpdateVisuals()
    {
        if (!this.IsNodeReady() || !CombatManager.Instance.IsInProgress)
            return;
        this._brandColorEnergyLabel?.SetTextAutoSize(this._energy.ToString());
        if (!this._isTween && this._body != null)
        {
            var nowPosition = this.Position;
            Tween tween = CreateTween();
            var delay = new Random().NextDouble();
            tween.TweenProperty(this, "position", new Vector2(nowPosition.X, nowPosition.Y - 4), 1).SetDelay(delay);
            tween.Finished += () =>
            {
                Tween tween2 = CreateTween();
                var rand = (double)new Random().Next(8, 13);
                tween2.TweenProperty(this, "position", new Vector2(nowPosition.X, nowPosition.Y + 3), rand / 10).SetEase(Tween.EaseType.InOut);
                tween2.TweenProperty(this, "position", new Vector2(nowPosition.X, nowPosition.Y - 4), rand / 10).SetEase(Tween.EaseType.InOut);
                tween2.SetLoops();
            };
            this._isTween = true;
        }
    }

    public void Flash(Color? flash)
    {
        
    }

    public void SetEnergy(int energy, Color? flash)
    {
        this._energy = energy;
        this.UpdateVisuals();
        if (flash != null)
            this.Flash(flash);
    }
}