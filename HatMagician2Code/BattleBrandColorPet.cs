using Godot;
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
	private TextureRect? _textureRect; // 实际显示的节点

	public override void _Ready()
	{
		base._Ready();
		this._brandColorEnergyLabel = this.GetNode<MegaLabel>((NodePath)"%PassiveAmount");
		this._body = this.GetNode<Node2D>((NodePath)"%Visuals");
		this._textureRect = (TextureRect?)this._body.GetChildren().FirstOrDefault(node => node is TextureRect { Visible: true });
		this._SubReady();
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

	public void Flash(bool flash)
	{
		// this.GlowColor = flash;
		// this.BlinkSpeed = 0.8f;
		// this.GlowThickness = 15f;
		// this.BlinkOnce();
		if (flash)
			this.StartPulseTween();
	}

	public void SetEnergy(int energy, bool flash = true)
	{
		this._energy = energy;
		this.UpdateVisuals();
		this.Flash(flash);
	}


	// AI补充
	[Export(PropertyHint.Range, "0.0,2.0,0.1")]
	public float GhostSpacing
	{
		get => _ghostSpacing;
		set
		{
			_ghostSpacing = value;
			SetShaderParameter("ghost_spacing", _ghostSpacing);
		}
	}

	private float _ghostSpacing = 1.5f; // 虚像放大倍率

	[Export(PropertyHint.Range, "0.1,10.0,0.1")]
	public float Speed { get; set; } = 1.0f; // 闪烁速度（进度增长速度）

	[Export(PropertyHint.Range, "0.1,5.0,0.1")]
	public float Duration { get; set; } = 1.0f; // 单次闪烁总时长

	[Export] public bool AutoPlay { get; set; } // 默认不自动播放

	[Export] public bool UseUnscaledTime { get; set; }

	private ShaderMaterial? _shaderMaterial;
	private double _progress;
	private bool _isPlaying;

	public void _SubReady()
	{
		this._shaderMaterial = this._textureRect?.Material as ShaderMaterial;

		if (_shaderMaterial == null)
		{
			GD.PushError("未找到 ShaderMaterial，请确保在 TextureRect 的 Material 属性中已挂载 ShaderMaterial");
			return;
		}

		SetShaderParameter("ghost_spacing", _ghostSpacing);
		SetShaderParameter("progress", 0.0f);

		if (AutoPlay)
		{
			StartPulse();
		}
	}

	private void SetShaderParameter(string name, Variant value)
	{
		_shaderMaterial?.SetShaderParameter(name, value);
	}

	// 开始一次闪烁
	public void StartPulse()
	{
		_progress = 0.0;
		_isPlaying = true;
		SetProcess(true);
	}

	// 停止闪烁
	public void StopPulse()
	{
		_isPlaying = false;
		SetProcess(false);
		this.SetProgress(0.0f);
	}

	public override void _Process(double delta)
	{
		if (!_isPlaying || _shaderMaterial == null) return;

		double dt = UseUnscaledTime ? delta / Engine.TimeScale : delta;
		_progress += dt * Speed / Duration;

		if (_progress >= 1.0)
		{
			_progress = 1.0;
			_isPlaying = false;
			SetProcess(false);
			// 闪烁结束，可选：触发回调
			// EmitSignal(SignalName.PulseFinished);
		}

		this.SetProgress((float)_progress);
	}

	// 使用 Tween 实现更平滑的控制
	public void StartPulseTween()
	{
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Quad);
		tween.SetEase(Tween.EaseType.Out);

		tween.TweenMethod(Callable.From<float>(SetProgress), 0.0f, 1.0f, Duration / Speed);
		// tween.Finished += () => EmitSignal(SignalName.PulseFinished);
	}

	private void SetProgress(float value)
	{
		var value2 = Math.Round(Math.Clamp(value, 0, 1), 3);
		_progress = value2;
		SetShaderParameter("progress", value2);
	}

	// [Signal]
	// public delegate void PulseFinishedEventHandler();
}
