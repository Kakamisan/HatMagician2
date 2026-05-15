using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.SceneControl;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Powers;

public class BrandPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public virtual BrandColor BaseBrandColor => BrandColor.None;
    protected virtual decimal BasePassiveVal => 0; // 基础被动值
    protected virtual decimal BaseEvokeVal => 0; // 基础刻印值
    protected virtual decimal BaseFusionVal => 0; // 基础叠色值

    public decimal PassiveVal => BasePassiveVal;
    public decimal EvokeVal => BaseEvokeVal;
    protected decimal FusionVal => BaseFusionVal;

    protected virtual string PassiveSfx => "";
    protected virtual string EvokeSfx => "";
    protected virtual string ChannelSfx => "";
    protected bool Evoked;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new("Passive", this.PassiveVal), new("Evoke", this.EvokeVal), new("Fusion", this.FusionVal)];

    // 刻印效果
    protected virtual async Task OnEvoke(HatMagician2Card? cardSource)
    {
        Log.Info("[   Hat2   ]OnEvoke:" + this.BaseBrandColor);
        this.Evoked = true;
        await Task.CompletedTask;
    }

    // 被动效果
    protected virtual async Task OnPassive()
    {
        Log.Info("[   Hat2   ]OnPassive:" + this.BaseBrandColor);
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
    public static async Task ApplyBrandPower(HatMagician2Card card, PlayerChoiceContext choiceContext, CardPlay play, BrandColor color)
    {
        // 检查
        if (color is BrandColor.None or > BrandColor.Rainbow)
            return;

        var oldPower = (BrandPower?)play.Target!.Powers.FirstOrDefault(p => p is BrandPower);

        // 实际应用的印记颜色
        var applyColor = color;

        // 叠色
        if (oldPower != null && (oldPower.BaseBrandColor & color) == 0)
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
                await PowerCmd.Apply<BrandRedPower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
            case BrandColor.Yellow:
                await PowerCmd.Apply<BrandYellowPower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
            case BrandColor.Blue:
                await PowerCmd.Apply<BrandBluePower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
            case BrandColor.Purple:
                await PowerCmd.Apply<BrandPurplePower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
            case BrandColor.Orange:
                await PowerCmd.Apply<BrandOrangePower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
            case BrandColor.White:
                await PowerCmd.Apply<BrandWhitePower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
            case BrandColor.Rainbow:
                await PowerCmd.Apply<BrandRainbowPower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
            case BrandColor.None:
            case BrandColor.Any:
            case BrandColor.All:
            default:
                throw new ArgumentOutOfRangeException();
        }

        // 是否触发叠色效果
        var newPower = (BrandPower?)play.Target!.Powers.FirstOrDefault(p => p is BrandPower);
        var isFusion = false;
        if (newPower != null && color != applyColor)
        {
            isFusion = true;
        }

        if (newPower != null)
        {
            await newPower.OnApply(card, isFusion);
        }

        // 其他杂项
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
    public static async Task ChainDamageCmd(BrandPower power, decimal damage, bool withDefaultVfx = true)
    {
        if (!power.Owner.IsAlive)
            return;
        if (power.Owner.CombatState == null)
            return;
        var enemies = power.Owner.CombatState.HittableEnemies.Where(c => c.Powers.Any(p => p is BrandPower)).ToList();
        if (withDefaultVfx)
        {
            foreach (var target1 in (IEnumerable<Creature>)enemies)
                VfxCmd.PlayOnCreature(target1, "vfx/vfx_attack_lightning");
            SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_passive");
        }

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemies, damage, ValueProp.Unpowered, power.Applier, null);
    }

    // 是否即将触发刻印
    public static bool WillEvoke(CardModel? cardSource, Creature? target)
    {
        return cardSource is HatMagician2Card card && card.IsEvokeCard() && target?.HasPower<BrandPower>() == true;
    }
}