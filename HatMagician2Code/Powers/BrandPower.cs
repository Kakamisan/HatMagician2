using System.Reflection;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HatMagician2.HatMagician2Code.Powers;

// public interface IBrandPower
// {
//     static HatMagician2BrandColor BaseBrandColor;
// }

public class BrandPower : HatMagician2Power
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected int BasePassiveVal;
    protected int BaseEvokeVal;
    protected int PassiveVal => BasePassiveVal;
    protected int EvokeVal => BaseEvokeVal;
    protected virtual string EvokeSfx => "";
    protected static HatMagician2BrandColor BaseBrandColor;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new("Passive", this.PassiveVal), new("Evoke", this.EvokeVal)];

    protected virtual async Task OnEvoke(HatMagician2Card? card)
    {
        this.OnSfx();
        var completedTask = Task.CompletedTask;
        await completedTask;
    }

    protected void OnSfx()
    {
        if (!this.Owner.IsAlive) return;
        this.Flash();
        if (this.EvokeSfx.Length > 0)
            SfxCmd.Play(this.EvokeSfx);
    }

    public static async Task ApplyBrandPower<T>(HatMagician2Card card, PlayerChoiceContext choiceContext, CardPlay play)
        where T : BrandPower
    {
        // todo 处理刻印 叠色等分支处理
        BrandPower? old = (BrandPower?)play.Target!.Powers.FirstOrDefault(p => p is BrandPower);
        if (old != null)
        {
            await old.OnEvoke(card);
            await PowerCmd.Remove(old);
        }
        // todo 怎么获取T的BaseBrandColor
        await PowerCmd.Apply<T>(choiceContext, play.Target!, 1, card.Owner.Creature, card);
    }
}