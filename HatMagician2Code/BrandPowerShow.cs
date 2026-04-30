using System.Globalization;
using Godot;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HatMagician2.HatMagician2Code;

public partial class BrandPowerShow : Node2D
{
	private TextureRect? _red;
	private TextureRect? _yellow;
	private TextureRect? _blue;
	private TextureRect? _purple;
	private TextureRect? _orange;
	private TextureRect? _white;

	private BattleLabel? _passiveAmount;
	private BattleLabel? _evokeAmount;

	private bool _isHover;

	private const string NodeName = "BrandPowerShow";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		// this._red = GetNode<TextureRect>((NodePath)"%Red");
		// this._yellow = GetNode<TextureRect>((NodePath)"%Yellow");
		// this._blue = GetNode<TextureRect>((NodePath)"%Blue");
		// this._purple = GetNode<TextureRect>((NodePath)"%Purple");
		// this._orange = GetNode<TextureRect>((NodePath)"%Orange");
		// this._white = GetNode<TextureRect>((NodePath)"%White");
		// this._passiveAmount = GetNode<BattleLabel>((NodePath)"%PassiveAmount");
		// this._evokeAmount = GetNode<BattleLabel>((NodePath)"%EvokeAmount");
	}

	// 应用印记
	public static void OnBrandApply(Creature? creature, BrandPower power)
	{
		Log.Info("[   Hat2   ]Show OnBrandApply");
		BrandPowerShow? node = GetOrInit(creature);

		node?.OnBrandApply(power);
	}

	// 删除印记
	public static void OnBrandRemove(Creature? creature)
	{
		Log.Info("[   Hat2   ]Show OnBrandRemove");
		BrandPowerShow? node = GetOrInit(creature);

		node?.OnBrandRemove();
	}

	// 指向（即将刻印）时显示刻印数值
	public static void OnHover(Creature? creature, bool evoke)
	{
		Log.Info("[   Hat2   ]Show OnHover:" + evoke);
		BrandPowerShow? node = GetOrInit(creature);

		node?.SetIsHover(evoke);
	}

	private void OnBrandApply(BrandPower power)
	{
		if (!this.IsNodeReady() || !CombatManager.Instance.IsInProgress)
			return;
		Log.Info("[   Hat2   ]Show OnBrandApply 2");
		this.Visible = true;
		// BrandColor color = power.BaseBrandColor;
		// this._red!.Visible = color == BrandColor.Red;
		// this._yellow!.Visible = color == BrandColor.Yellow;
		// this._blue!.Visible = color == BrandColor.Blue;
		// this._purple!.Visible = color == BrandColor.Purple;
		// this._orange!.Visible = color == BrandColor.Orange;
		// this._white!.Visible = color == BrandColor.White;
		Log.Info("[   Hat2   ]Show OnBrandApply 3");
		// this._passiveAmount!.SetTextAutoSize(((int)power.PassiveVal).ToString());
		// this._evokeAmount!.SetTextAutoSize(((int)power.EvokeVal).ToString());
		Log.Info("[   Hat2   ]Show OnBrandApply 4");
		this.UpdateAmountShow();
	}

	private void OnBrandRemove()
	{
		this.Visible = false;
	}

	private void UpdateAmountShow()
	{
		// this._passiveAmount!.Visible = !this._isHover;
		// this._evokeAmount!.Visible = this._isHover;
	}

	private void SetIsHover(bool evoke)
	{
		this._isHover = evoke;
		this.UpdateAmountShow();
	}

	private static BrandPowerShow? GetOrInit(Creature? creature)
	{
		if (creature == null)
			return null;
		NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
		if (creatureNode == null)
			return null;
		BrandPowerShow? node = null;
		if (creatureNode.HasNode(NodeName))
			node = creatureNode.GetNode<BrandPowerShow>(NodeName);
		// 初始化
		if (node == null)
		{
			PackedScene scene = GD.Load<PackedScene>("res://HatMagician2/scenes/brand_power_show.tscn");
			if (scene != null)
			{
				BrandPowerShow? newNode = scene.Instantiate() as BrandPowerShow;
				if (newNode != null)
				{
					Log.Info("[   Hat2   ]Show GetOrInit");
					newNode.Name = NodeName;
					creatureNode.AddChild(newNode);
					node = newNode;
					Log.Info("[   Hat2   ]Show GetOrInit 2");
				}
			}
		}

		return node;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
