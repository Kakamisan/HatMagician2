using Godot;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HatMagician2.HatMagician2Code.SceneControl;

public partial class BrandPowerShow : NCreatureVisuals
{
	private TextureRect? _red;
	private TextureRect? _yellow;
	private TextureRect? _blue;
	private TextureRect? _purple;
	private TextureRect? _orange;
	private TextureRect? _white;
	private TextureRect? _rainbow;

	private BattleLabel? _passiveAmount;
	private BattleLabel? _evokeAmount;

	private bool _isHover;
	private decimal PassiveVal => _brandPower?.PassiveVal ?? 0;
	private decimal EvokeVal => _brandPower?.EvokeVal ?? 0;
	private BrandColor _brandColor;

	private BrandPower? _brandPower;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		this._red = GetNode<TextureRect>((NodePath)"%Red");
		this._yellow = GetNode<TextureRect>((NodePath)"%Yellow");
		this._blue = GetNode<TextureRect>((NodePath)"%Blue");
		this._purple = GetNode<TextureRect>((NodePath)"%Purple");
		this._orange = GetNode<TextureRect>((NodePath)"%Orange");
		this._white = GetNode<TextureRect>((NodePath)"%White");
		this._rainbow = GetNode<TextureRect>((NodePath)"%Rainbow");
		this._passiveAmount = GetNode<BattleLabel>((NodePath)"%PassiveAmount");
		this._evokeAmount = GetNode<BattleLabel>((NodePath)"%EvokeAmount");
	}

	// 应用印记
	public static void OnBrandApply(Creature? creature, BrandPower power)
	{
		Log.Info("[   Hat2   ]Show OnBrandApply");
		BrandPowerShow? node = GetOrInitBrandPowerShow(creature);

		node?.OnBrandApply(power);
	}

	// 删除印记
	public static void OnBrandRemove(Creature? creature)
	{
		Log.Info("[   Hat2   ]Show OnBrandRemove");
		BrandPowerShow? node = GetOrInitBrandPowerShow(creature);

		node?.OnBrandRemove();
	}

	// 指向（即将刻印）时显示刻印数值
	public static void OnHover(Creature? creature, bool evoke)
	{
		Log.Info("[   Hat2   ]Show OnHover:" + evoke);
		BrandPowerShow? node = GetOrInitBrandPowerShow(creature);

		node?.SetIsHover(evoke);
	}

	// 更新数值
	public static void OnUpdate(Creature? creature)
	{
		Log.Info("[   Hat2   ]Show OnUpdate");
		BrandPowerShow? node = GetOrInitBrandPowerShow(creature);

		node?.UpdateAmountShow();
	}

	private void OnBrandApply(BrandPower power)
	{
		if (!this.IsNodeReady() || !CombatManager.Instance.IsInProgress)
			return;
		this.Visible = true;
		this._brandPower = power;
		BrandColor color = power.BaseBrandColor;
		this._brandColor = color;
		this._red!.Visible = color == BrandColor.Red;
		this._yellow!.Visible = color == BrandColor.Yellow;
		this._blue!.Visible = color == BrandColor.Blue;
		this._purple!.Visible = color == BrandColor.Purple;
		this._orange!.Visible = color == BrandColor.Orange;
		this._white!.Visible = color == BrandColor.White;
		this._rainbow!.Visible = color == BrandColor.Rainbow;
		this.UpdateAmountShow();
	}

	private string GetEvokeValShow()
	{
		return this._brandColor switch
		{
			BrandColor.Red => "×" + this.GetRedMulti(),
			BrandColor.Orange => "Aoe",
			BrandColor.White => "Draw " + (int)this.EvokeVal,
			_ => ((int)this.EvokeVal).ToString()
		};
	}

	private int GetRedMulti()
	{
		var multi = MultiDamagePower.GetAmount(this._brandPower?.Owner);
		multi = multi > 0 ? multi - 1 : 0;
		return (int)this.EvokeVal + multi;
	}

	private void OnBrandRemove()
	{
		this.Visible = false;
		this._isHover = false;
	}

	private void UpdateAmountShow()
	{
		this._passiveAmount!.TrySetText(((int)PassiveVal).ToString());
		this._evokeAmount!.TrySetText(this.GetEvokeValShow());
		this._passiveAmount!.Visible = !this._isHover && this.PassiveVal > 0;
		this._evokeAmount!.Visible = this._isHover && this.EvokeVal > 0;
	}

	private void SetIsHover(bool evoke)
	{
		this._isHover = evoke;
		this.UpdateAmountShow();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private const string BrandPowerShowNodeName = "BrandPowerShow";

	private static BrandPowerShow? GetOrInitBrandPowerShow(Creature? creature)
	{
		if (creature == null)
			return null;
		NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
		if (creatureNode == null)
			return null;
		BrandPowerShow? node = null;
		if (creatureNode.HasNode(BrandPowerShowNodeName))
			node = creatureNode.GetNode<BrandPowerShow>(BrandPowerShowNodeName);
		// 初始化
		if (node != null) return node;
		PackedScene scene = GD.Load<PackedScene>("res://HatMagician2/scenes/brand_power_show.tscn");
		BrandPowerShow? newNode = scene.Instantiate() as BrandPowerShow;
		if (newNode is null) return node;
		// Log.Info("[   Hat2   ]Show GetOrInit");
		newNode.Name = BrandPowerShowNodeName;
		const int xOffset = 0;
		const int yOffset = 0;
		newNode.Position = new Vector2(creatureNode.Visuals.VfxSpawnPosition.Position.X - xOffset, creatureNode.Visuals.VfxSpawnPosition.Position.Y + yOffset);
		//Log.Info("[   Hat2   ]Show GetOrInit Pos:" + creatureNode.Visuals.VfxSpawnPosition.Position.X + "," + creatureNode.Visuals.VfxSpawnPosition.Position.Y);
		creatureNode.AddChild(newNode);
		node = newNode;

		return node;
	}
}
