using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Character;

public class HatMagician2Mgr : CustomSingletonModel
{
    public HatMagician2Mgr() : base(true, false)
    {
        Instance = this;
        _playerEnergyStates = new Dictionary<Player, BrandColorEnergyState>();
    }

    public static HatMagician2Mgr? Instance { get; private set; }

    private readonly Dictionary<Player, BrandColorEnergyState> _playerEnergyStates; // 绘色能量管理

    // 战前清空绘色能量
    public override async Task BeforeCombatStart()
    {
        foreach (var state in this._playerEnergyStates.Values)
        {
            state.BeforeCombatStart();
        }

        await base.BeforeCombatStart();
    }

    // 初始化/获取这个玩家的state
    private BrandColorEnergyState InitState(Player player)
    {
        var state = this._playerEnergyStates.GetValueOrDefault(player);
        if (state != null)
            return state;
        state = new BrandColorEnergyState(player);
        this._playerEnergyStates.Add(player, state);
        return state;
    }

    // 外部获取state
    public BrandColorEnergyState GetState(Player player)
    {
        return this.InitState(player);
    }

    // 获得绘色能量
    public static async Task AddEnergy(Player player, int amount, BrandColor color = BrandColor.Any)
    {
        if (Instance == null) return;
        var state = Instance.InitState(player);

        var applyColor = color;
        if (applyColor == BrandColor.Any)
        {
            applyColor = (BrandColor)player.RunState.Rng.CombatEnergyCosts.NextInt((int)BrandColor.Red, (int)BrandColor.White);
            state.AddEnergy(applyColor, amount);
            return;
        }

        if (applyColor is BrandColor.All or BrandColor.Rainbow)
        {
            var c = BrandColor.Red;
            while (c < BrandColor.Rainbow)
            {
                state.AddEnergy(c, amount);
                c++;
            }

            return;
        }

        state.AddEnergy(applyColor, amount);
        await Task.CompletedTask;
    }

    public static Decimal ModifyBrandColorCost(ICombatState combatState, HatMagician2Card card, Decimal originalCost)
    {
        if (originalCost < 0M)
            return originalCost;
        Decimal modifiedCost = originalCost;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyBrandColorCost(card, modifiedCost, out modifiedCost);
            }
        }

        return modifiedCost;
    }

    public static bool TryModifyBrandColorCostWithHooks(HatMagician2Card card, ICombatState state, out Decimal hookModifiedCost)
    {
        hookModifiedCost = card.BaseBrandColorCost;
        bool flag = false;
        foreach (AbstractModel iterateHookListener in state.IterateHookListeners())
            if (iterateHookListener is IHatMagician2AbstractModel it)
            {
                flag |= it.TryModifyBrandColorCost(card, hookModifiedCost, out hookModifiedCost);
            }

        return flag;
    }

    public static bool HasEnoughEnergy(Player player, BrandColor color, decimal cost)
    {
        if (color == BrandColor.None) return true;
        if (Instance == null) return true;
        var eState = Instance._playerEnergyStates.GetValueOrDefault(player);
        if (eState == null) return false;
        return cost <= eState.BrandColorEnergyMap[color];
    }
}