using BaseLib.Abstracts;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

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
            await state.AddEnergy(applyColor, amount);
            return;
        }

        if (applyColor is BrandColor.All or BrandColor.Rainbow)
        {
            var c = BrandColor.Red;
            while (c < BrandColor.Rainbow)
            {
                await state.AddEnergy(c, amount);
                c++;
            }

            return;
        }

        await state.AddEnergy(applyColor, amount);
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

    public static int GetBrandColorTypeCnt(Player player)
    {
        if (Instance == null) return 0;
        var state = Instance.InitState(player);
        return state.GetBrandColorTypeCnt();
    }

    // 印记激活动态数值
    public static decimal ModifyEvokeVal(ICombatState combatState, BrandPower power, decimal originVal)
    {
        decimal modifiedVal = originVal;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyEvokeValMulti(power, modifiedVal, out modifiedVal);
            }
        }

        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyEvokeValAdditive(power, modifiedVal, out modifiedVal);
            }
        }

        return modifiedVal;
    }

    // 印记激活动态数值2
    public static decimal ModifyEvokeVal2(ICombatState combatState, BrandPower power, decimal originVal)
    {
        decimal modifiedVal = originVal;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyEvokeVal2Multi(power, modifiedVal, out modifiedVal);
            }
        }

        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyEvokeVal2Additive(power, modifiedVal, out modifiedVal);
            }
        }

        return modifiedVal;
    }

    // 印记被动动态数值
    public static decimal ModifyPassiveVal(ICombatState combatState, BrandPower power, decimal originVal)
    {
        decimal modifiedVal = originVal;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyPassiveValMulti(power, modifiedVal, out modifiedVal);
            }
        }

        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyPassiveValAdditive(power, modifiedVal, out modifiedVal);
            }
        }

        return modifiedVal;
    }

    // 印记叠色动态数值
    public static decimal ModifyFusionVal(ICombatState combatState, BrandPower power, decimal originVal)
    {
        decimal modifiedVal = originVal;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyFusionValMulti(power, modifiedVal, out modifiedVal);
            }
        }

        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyFusionValAdditive(power, modifiedVal, out modifiedVal);
            }
        }

        return modifiedVal;
    }

    // 获得绘色派发
    public static async Task AfterAddEnergy(Player player, int amount, BrandColor color)
    {
        if (player.Creature.CombatState == null) return;
        foreach (AbstractModel iterateHookListener in player.Creature.CombatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                await iterate.AfterAddEnergy(player, amount, color);
            }
        }
    }

    // 消耗绘色派发
    public static async Task AfterSpendEnergy(Player player, int amount, BrandColor color)
    {
        if (player.Creature.CombatState == null) return;
        foreach (AbstractModel iterateHookListener in player.Creature.CombatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                await iterate.AfterSpendEnergy(player, amount, color);
            }
        }
    }

    // 影响连锁伤害数值
    public static decimal ModifyChainDamage(Creature? target, decimal damage, ValueProp props, Creature? applier, CardModel? card, ICombatState combatState)
    {
        decimal modifiedVal = damage;
        foreach (AbstractModel iterateHookListener in combatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                iterate.TryModifyChainDamageAdditive(target, modifiedVal, props, applier, card, out modifiedVal);
            }
        }

        return modifiedVal;
    }

    // 阴郁伤害派发
    public static async Task AfterGloomyDamage(Creature target, decimal damage, Creature? dealer)
    {
        if (target.CombatState == null) return;
        foreach (AbstractModel iterateHookListener in target.CombatState.IterateHookListeners())
        {
            if (iterateHookListener is IHatMagician2AbstractModel iterate)
            {
                await iterate.AfterGloomyDamage(target, damage, dealer);
            }
        }
    }
}