using System.Reflection;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;

namespace HatMagician2.HatMagician2Code.Powers;

// public interface IBrandPower
// {
//     static HatMagician2BrandColor BaseBrandColor;
// }

public class BrandPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected int BasePassiveVal; // 基础被动值
    protected int BaseEvokeVal; // 基础刻印值
    protected int BaseFusionVal; // 基础叠色值
    protected BrandColor BaseBrandColor;
    protected int PassiveVal => BasePassiveVal;
    protected int EvokeVal => BaseEvokeVal;
    protected int FusionVal => BaseFusionVal;
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
        this.OnSfx(this.EvokeSfx);
        this.Evoked = true;
        await Task.CompletedTask;
    }
    
    // 被动效果
    protected virtual async Task OnPassive()
    {
        Log.Info("[   Hat2   ]OnPassive:" + this.BaseBrandColor);
        this.OnSfx(this.PassiveSfx);
        await Task.CompletedTask;
    }

    // 叠色效果
    protected virtual async Task OnFusion(HatMagician2Card? cardSource)
    {
        Log.Info("[   Hat2   ]OnFusion:" + this.BaseBrandColor);
        this.OnSfx(this.ChannelSfx);
        await Task.CompletedTask;
    }

    // 移除之后的处理
    public override Task AfterRemoved(Creature oldOwner)
    {
        Log.Info("[   Hat2   ]OnRemoved:" + this.BaseBrandColor);
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
    public static async Task ApplyBrandPower(HatMagician2Card card, PlayerChoiceContext choiceContext, CardPlay play,
        BrandColor color)
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
            case BrandColor.Purple:
            case BrandColor.Orange:
            case BrandColor.White:
            case BrandColor.Rainbow:
                break;
            default:
                await PowerCmd.Apply<BrandYellowPower>(choiceContext, play.Target!, 1, card.Owner.Creature, card, true);
                break;
        }

        // 叠色效果
        // if (color != applyColor)
        // {
        // }
        var newPower = (BrandPower?)play.Target!.Powers.FirstOrDefault(p => p is BrandPower);
        if (newPower != null)
        {
            await newPower.OnFusion(card);
        }

        // 其他杂项
        card.IsBrandApplied = true;
    }
}