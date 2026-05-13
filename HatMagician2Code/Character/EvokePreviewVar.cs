using HatMagician2.HatMagician2Code.Powers;
using HatMagician2.HatMagician2Code.SceneControl;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HatMagician2.HatMagician2Code.Character;

// 卡牌指向敌人时 计算是否即将触发刻印 用于更新印记的显示
public class EvokePreviewVar() : DynamicVar(DefaultName, 0)
{
    private const string DefaultName = "Evoke";
    private bool _isEvokePreview;
    private Creature? _lastTarget;

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        if (target == null && this._lastTarget != null)
        {
            BrandPowerShow.OnHover(this._lastTarget, false);
            this._lastTarget = null;
            this._isEvokePreview = false;
        }

        if (target != null && BrandPower.WillEvoke(card, target))
        {
            this._lastTarget = target;
            this._isEvokePreview = true;
            BrandPowerShow.OnHover(this._lastTarget, this._isEvokePreview);
        }
    }
}