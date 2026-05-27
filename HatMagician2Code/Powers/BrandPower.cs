using BaseLib.Extensions;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.SceneControl;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool FakeDebuff => true;

    public virtual BrandColor BaseBrandColor => BrandColor.None;
    protected virtual decimal BasePassiveVal => 0; // 基础被动值
    protected virtual decimal BaseEvokeVal => 0; // 基础刻印值1
    protected virtual decimal BaseEvokeVal2 => 0; // 基础刻印值2
    protected virtual decimal BaseFusionVal => 0; // 基础叠色值

    public decimal PassiveVal => this.GetDynamicVar("Passive").BaseValue;
    public decimal EvokeVal => this.GetDynamicVar("Evoke").BaseValue;
    public decimal EvokeVal2 => this.GetDynamicVar("Evoke2").BaseValue;
    public decimal FusionVal => this.GetDynamicVar("Fusion").BaseValue;

    protected virtual string PassiveSfx => "";
    protected virtual string EvokeSfx => "";
    protected virtual string ChannelSfx => "";
    protected bool Evoked;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new("Passive", this.BasePassiveVal), new("Evoke", this.BaseEvokeVal), new("Evoke2", this.BaseEvokeVal2), new("Fusion", this.BaseFusionVal)];

    private bool _thisTurnIsTriggeredPassive; // 用于辅助判断死亡时是否需要触发一次被动（触发连锁伤害）

    public override Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == this.Owner.Side)
        {
            this._thisTurnIsTriggeredPassive = false;
        }

        return base.AfterSideTurnEndLate(choiceContext, side, participants);
    }

    // 刻印效果
    protected virtual async Task OnEvoke(HatMagician2Card? cardSource)
    {
        Log.Info("[   Hat2   ]OnEvoke:" + this.BaseBrandColor);
        this.Evoked = true;
        await Task.CompletedTask;
    }

    // 被动效果
    protected virtual async Task OnPassive(bool setFlag = true)
    {
        Log.Info("[   Hat2   ]OnPassive:" + this.BaseBrandColor);
        if (setFlag)
        {
            this._thisTurnIsTriggeredPassive = true;
        }

        await Task.CompletedTask;
    }

    // 叠色效果
    protected virtual async Task OnFusion(HatMagician2Card? cardSource)
    {
        Log.Info("[   Hat2   ]OnFusion:" + this.BaseBrandColor);
        await Task.CompletedTask;
    }

    // 赋予效果 
    protected virtual async Task OnApply(HatMagician2Card? cardSource, bool isFusion = false)
    {
        Log.Info("[   Hat2   ]OnApply:" + this.BaseBrandColor);
        this.OnSfx(this.ChannelSfx);
        if (isFusion)
        {
            await this.OnFusion(cardSource);
        }

        BrandPowerShow.OnBrandApply(this.Owner, this);
        await Task.CompletedTask;
    }

    // 移除之后的处理
    public override Task AfterRemoved(Creature oldOwner)
    {
        Log.Info("[   Hat2   ]OnRemoved:" + this.BaseBrandColor);
        BrandPowerShow.OnBrandRemove(oldOwner);
        return base.AfterRemoved(oldOwner);
    }

    // 图标闪烁+音效
    private void OnSfx(string sfx = "")
    {
        if (!this.Owner.IsAlive) return;
        this.Flash();
        if (sfx.Length > 0)
            SfxCmd.Play(sfx);
    }

    // 应用印记的逻辑
    public static async Task ApplyBrandPower(HatMagician2Card card, PlayerChoiceContext choiceContext, CardPlay play, BrandColor color) =>
        await ApplyBrandPower(card, card.Owner.Creature, choiceContext, play.Target!, color);

    public static async Task ApplyBrandPower(HatMagician2Card card, PlayerChoiceContext choiceContext, Creature target, BrandColor color) =>
        await ApplyBrandPower(card, card.Owner.Creature, choiceContext, target, color);

    public static async Task ApplyBrandPower(HatMagician2Card? card, Creature applier, PlayerChoiceContext choiceContext, Creature target, BrandColor color)
    {
        // 检查
        if (color is BrandColor.None or > BrandColor.Rainbow)
            return;

        var oldPower = (BrandPower?)target.Powers.FirstOrDefault(p => p is BrandPower);

        // 实际应用的印记颜色
        var applyColor = color;

        // 叠色
        if (oldPower != null && (oldPower.BaseBrandColor & color) == 0 && (color & (color - 1)) == 0 && (card == null || !card.Keywords.Contains(HatMagician2Keywords.Erosion)))
        {
            applyColor = oldPower.BaseBrandColor | color;
        }

        // 触发刻印效果
        if (oldPower != null)
        {
            await oldPower.OnEvoke(card);
            await PowerCmd.Remove(oldPower);
        }

        // 应用新印记
        switch (applyColor)
        {
            case BrandColor.Red:
                await PowerCmd.Apply<BrandRedPower>(choiceContext, target, 1, applier, card, true);
                break;
            case BrandColor.Yellow:
                await PowerCmd.Apply<BrandYellowPower>(choiceContext, target, 1, applier, card, true);
                break;
            case BrandColor.Blue:
                await PowerCmd.Apply<BrandBluePower>(choiceContext, target, 1, applier, card, true);
                break;
            case BrandColor.Purple:
                await PowerCmd.Apply<BrandPurplePower>(choiceContext, target, 1, applier, card, true);
                break;
            case BrandColor.Orange:
                await PowerCmd.Apply<BrandOrangePower>(choiceContext, target, 1, applier, card, true);
                break;
            case BrandColor.White:
                await PowerCmd.Apply<BrandWhitePower>(choiceContext, target, 1, applier, card, true);
                break;
            case BrandColor.Rainbow:
                await PowerCmd.Apply<BrandRainbowPower>(choiceContext, target, 1, applier, card, true);
                break;
            case BrandColor.None:
            case BrandColor.Any:
            case BrandColor.All:
            default:
                throw new ArgumentOutOfRangeException();
        }

        // 是否触发叠色效果
        var newPower = (BrandPower?)target.Powers.FirstOrDefault(p => p is BrandPower);
        var isFusion = newPower != null && color != applyColor;

        if (newPower != null)
        {
            await newPower.OnApply(card, isFusion);
        }

        // 其他杂项
        if (card != null)
            card.IsBrandApplied = true;
    }

    // 应用刻印效果
    public static async Task ApplyBrandEvoke(HatMagician2Card card, PlayerChoiceContext choiceContext, CardPlay play)
    {
        var oldPower = (BrandPower?)play.Target!.Powers.FirstOrDefault(p => p is BrandPower);

        // 触发刻印效果
        if (oldPower != null)
        {
            await oldPower.OnEvoke(card);
            await PowerCmd.Remove(oldPower);
        }
    }

    // 连锁伤害
    public static async Task ChainDamageCmd(BrandPower power, decimal damage, bool withDefaultVfx = true, int cnt = 1)
    {
        await ChainDamageCmd(power.Owner, damage, power.Applier, null, withDefaultVfx, cnt);
    }

    public static async Task ChainDamageCmd(Creature target, decimal damage, Creature? applier, CardModel? card, bool withDefaultVfx = true, int cnt = 1)
    {
        if (target.CombatState == null) return;
        var enemies = target.CombatState.GetTeammatesOf(target).Where(c => c.Powers.Any(p => p is BrandPower) && c.IsAlive).ToList();
        // var modifyChainDamage = HatMagician2Mgr.ModifyChainDamage(target, damage, ValueProp.Unpowered, applier, card, target.CombatState);
        for (int i = 0; i < cnt; i++)
        {
            var enemies2 = enemies.Where(c => c.IsAlive).ToList();
            if (withDefaultVfx)
            {
                foreach (var target1 in (IEnumerable<Creature>)enemies2)
                    VfxCmd.PlayOnCreature(target1, "vfx/vfx_attack_lightning");
                SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_passive");
            }

            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemies2, damage, ValueProp.Unpowered, applier, card);
        }

        await Task.CompletedTask;
    }

    // 是否即将触发刻印
    public static bool WillEvoke(CardModel? cardSource, Creature? target)
    {
        return cardSource is HatMagician2Card card && card.IsEvokeCard() && target?.HasPower<BrandPower>() == true;
    }

    // 在死亡动画前移除
    public override async Task BeforeDeath(Creature creature)
    {
        if (this.Owner == creature)
        {
            // 怪物在他的回合死亡 且未触发过被动 则触发一次被动
            if (!this._thisTurnIsTriggeredPassive && this.CombatState.CurrentSide == this.Owner.Side)
                await this.OnPassive();
            //await PowerCmd.Remove(this);
        }

        await base.BeforeDeath(creature);
    }

    // 使用印记被动效果
    public static async Task UsePassiveCmd(Creature target, CardModel card, int cnt = 1)
    {
        var power = (BrandPower?)target.Powers.FirstOrDefault(p => p is BrandPower);
        if (power == null) return;
        switch (power.BaseBrandColor)
        {
            case BrandColor.Red:
                await BrandRedPower.UsePassive(power, card, cnt); break;
            case BrandColor.Yellow:
                await BrandYellowPower.UsePassive(power, card, cnt); break;
            case BrandColor.Blue:
                await BrandBluePower.UsePassive(power, card, cnt); break;
            case BrandColor.Purple:
                await BrandPurplePower.UsePassive(power, card, cnt); break;
            case BrandColor.Orange:
                await BrandOrangePower.UsePassive(power, card, cnt); break;
            case BrandColor.White:
                await BrandWhitePower.UsePassive(power, card, cnt); break;
        }

        await Task.CompletedTask;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        this.GetDynamicVar("Passive").BaseValue = this.GetPassiveValWithModifiers();
        this.GetDynamicVar("Evoke").BaseValue = this.GetEvokeValWithModifiers();
        this.GetDynamicVar("Evoke2").BaseValue = this.GetEvokeVal2WithModifiers();
        this.GetDynamicVar("Fusion").BaseValue = this.FusionVal;
        return base.AfterApplied(applier, cardSource);
    }

    // 更新各个Dynamic
    public override Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (power is HatMagician2Power { HasChangeBrandValEffect: true })
        {
            // 更新Evoke
            this.GetDynamicVar("Passive").BaseValue = this.GetPassiveValWithModifiers();
            this.GetDynamicVar("Evoke").BaseValue = this.GetEvokeValWithModifiers();
            this.GetDynamicVar("Evoke2").BaseValue = this.GetEvokeVal2WithModifiers();
            BrandPowerShow.OnUpdate(this.Owner);
        }

        return base.AfterModifyingPowerAmountReceived(power);
    }

    // 设置成接收事件
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (canonicalPower is HatMagician2Power { HasChangeBrandValEffect: true })
        {
            return true;
        }

        // other ...
        return false;
    }

    // 能力修改印记的数值
    public decimal GetEvokeValWithModifiers()
    {
        var change = HatMagician2Mgr.ModifyEvokeVal(this.CombatState, this, this.BaseEvokeVal);
        return change;
    }

    public decimal GetEvokeVal2WithModifiers()
    {
        var change = HatMagician2Mgr.ModifyEvokeVal2(this.CombatState, this, this.BaseEvokeVal2);
        return change;
    }

    public decimal GetPassiveValWithModifiers()
    {
        var change = HatMagician2Mgr.ModifyPassiveVal(this.CombatState, this, this.BasePassiveVal);
        return change;
    }
}