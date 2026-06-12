using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;

namespace HatMagician2.HatMagician2Code.Character;

public class CardStateSingleton : CustomSingletonModel, IHatMagician2AbstractModel
{
    public CardStateSingleton() : base(HookType.Combat)
    {
        _instance = this;
    }

    private static CardStateSingleton? _instance;

    private readonly Dictionary<Player, int> _brandColorCostCnt = new();

    public async Task AfterAddEnergy(Player player, int amount, BrandColor color)
    {
        this._brandColorCostCnt.TryAdd(player, 0);
        this._brandColorCostCnt[player] += amount;
        await Task.CompletedTask;
    }

    public static int GetColorCost(Player player)
    {
        return _instance != null ? _instance._brandColorCostCnt.GetValueOrDefault(player, 0) : 0;
    }

    // 战前战后重置数据
    public override async Task BeforeCombatStart()
    {
        this._brandColorCostCnt.Clear();
        await base.BeforeCombatStart();
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await this.BeforeCombatStart();
    }
}