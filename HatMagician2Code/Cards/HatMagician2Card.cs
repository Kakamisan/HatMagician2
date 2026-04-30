using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HarmonyLib;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;
using HatMagician2.HatMagician2Code.Powers;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

public abstract class HatMagician2Card(int cost, CardType type, CardRarity rarity, TargetType target) : CustomCardModel(cost, type, rarity, target), IHatMagician2AbstractModel
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => (IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of full art: 250x350
    //Smaller variant of normal art: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => (IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    public override string BetaPortraitPath => (IsTest ? "card.png" : $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    // 设置成test的话使用通用的测试卡图
    protected bool IsTest = false;

    // 基础绘色消耗
    public int BaseBrandColorCost = -1;

    // 变化后的绘色消耗
    public int BrandColorCost => this.BaseBrandColorCost;

    // 绘色消耗类型
    public BrandColor BaseBrandColor = BrandColor.None;

    // 绘色X费
    public bool HasBrandColorCostX = false;

    // 印记提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var hasExtra = false;
            IEnumerable<IHoverTip> baseTips =
            [
                HoverTipFactory.FromPower<BrandPower>()
            ];
            if (this.BaseBrandColor is > BrandColor.None and <= BrandColor.Rainbow)
                hasExtra = true;
            switch (this.BaseBrandColor)
            {
                case BrandColor.Red:
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandRedPower>());
                    break;
                case BrandColor.Blue:
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandBluePower>());
                    break;
                case BrandColor.Yellow:
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandYellowPower>());
                    break;
                case BrandColor.Orange:
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandOrangePower>());
                    break;
            }

            return hasExtra ? baseTips : [];
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => ((IEnumerable<DynamicVar>)[new EvokePreviewVar()]).Concat(this.Hat2ExtraCanonicalVars);

    protected virtual IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];

    /*已改成patch原版能量检查
    protected override bool IsPlayable => this.CheckBrandColorResource();

    // 检查绘色消耗
    public bool CheckBrandColorResource()
    {
        if (this.BaseBrandColor == HatMagician2BrandColor.None)
            return true;
        if (this.BrandColorCost <= 0)
            return true;
        PaletteBottle? relic = this.Owner?.GetRelic<PaletteBottle>();
        if (relic != null)
            return relic.HasEnoughEnergy(this.BaseBrandColor, this.BrandColorCost);
        return false;
    }
    */

    // 获取经过能力等修改后的绘色消耗
    public int GetBrandColorCostWithModifiers()
    {
        if (this.HasBrandColorCostX)
        {
            PaletteBottle? relic = this.Owner.GetRelic<PaletteBottle>();
            return relic == null ? 0 : relic.BrandColorEnergyMap[this.BaseBrandColor];
        }

        CardPile? pile = this.Pile;
        return pile != null && pile.IsCombatPile && this.CombatState != null
            ? (int)PaletteBottle.ModifyBrandColorCost(this.CombatState, this, this.BrandColorCost)
            : this.BrandColorCost;
    }

    // 下次攻击伤害变为N倍 只在火焰印记被刻印 且刻印时打出的是攻击牌时设置此值
    // 打出后复原此值
    public decimal NextPlayMulti = 1;
    public void SetNextPlayMulti(decimal value) => this.NextPlayMulti = value;
    public bool IsBrandApplied; // 是否已应用印记效果

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        return cardSource == this && MultiDamagePower.IsTriggerMulti(cardSource)
            ? this.NextPlayMulti
            : base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this)
        {
            this.IsBrandApplied = false;
            this.NextPlayMulti = 1;
        }

        return base.AfterCardPlayed(choiceContext, cardPlay);
    }

    // 是否可触发刻印的卡
    public bool IsEvokeCard()
    {
        return this is { BaseBrandColor: not BrandColor.None and < BrandColor.Rainbow } || this.CanonicalKeywords.Contains(HatMagician2Keywords.Evoke);
    }
}