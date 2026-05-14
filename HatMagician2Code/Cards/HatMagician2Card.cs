using BaseLib.Abstracts;
using BaseLib.Extensions;
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
    public override string CustomPortraitPath => (this.IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of full art: 250x350
    //Smaller variant of normal art: 250x190
    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => (this.IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();
    public override string BetaPortraitPath => (this.IsTest ? "card.png" : $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    // 设置成test的话使用通用的测试卡图
    protected virtual bool IsTest => false;

    // 绘色消耗类型
    public virtual BrandColor BaseBrandColor => BrandColor.None;

    // 绘色X费
    public virtual bool HasBrandColorCostX => false;

    // 基础绘色消耗
    public virtual int BaseBrandColorCost => -1;

    // 变化后的绘色消耗
    public int BrandColorCost => (int)this.GetDynamicVar(BrandColorCostVar.DefaultName).BaseValue;

    // 通用印记Tips
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
                case BrandColor.Purple:
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandPurplePower>());
                    break;
                case BrandColor.White:
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandWhitePower>());
                    break;
            }

            return (hasExtra ? baseTips : []).Concat(this.Hat2ExtraHoverTips);
        }
    }

    // 子类自己的Tips
    protected virtual IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        ((IEnumerable<DynamicVar>)[new EvokePreviewVar(), new BrandColorCostVar(this.BaseBrandColorCost)]).Concat(this.Hat2ExtraCanonicalVars);

    // 子类自己的Vars
    protected virtual IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => ((IEnumerable<CardKeyword>)[]).Concat(this.Hat2CanonicalKeywords);

    // 子类自己的Keyword
    protected virtual IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];

    protected override HashSet<CardTag> CanonicalTags
    {
        get
        {
            HashSet<CardTag> set = [];
            set.UnionWith(this.Hat2CanonicalTags);
            return set;
        }
    }

    // 子类自己的Tag
    protected virtual HashSet<CardTag> Hat2CanonicalTags => [];

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
            BrandColorEnergyState state = BrandColorEnergyMgr.Instance.GetState(this.Owner);
            return state.BrandColorEnergyMap[this.BaseBrandColor];
        }

        CardPile? pile = this.Pile;
        return pile != null && pile.IsCombatPile && this.CombatState != null
            ? (int)BrandColorEnergyMgr.ModifyBrandColorCost(this.CombatState, this, this.BrandColorCost)
            : this.BrandColorCost;
    }

    // 战斗状态相关
    // 下次攻击伤害变为N倍 只在火焰印记被刻印 且刻印时打出的是攻击牌时设置此值
    // 打出后复原此值
    public decimal NextPlayMulti = 1;
    public void SetNextPlayMulti(decimal value) => this.NextPlayMulti = value;
    public bool IsBrandApplied; // 是否已应用印记效果（判断是否要触发火焰印记N倍伤害 用于预览计算伤害）
    public bool NextCannotCost; // 是否无法消耗绘色（消耗绘色可打出额外效果 无法消耗则不能打出）
    public virtual bool HasFreeBrandApply => false; // 是否有不消耗绘色即可打出印记的效果

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
            this.NextCannotCost = false;
        }

        return base.AfterCardPlayed(choiceContext, cardPlay);
    }

    // 是否可触发刻印的卡
    public bool IsEvokeCard()
    {
        return this.HasFreeBrandApply
               || this is { BaseBrandColor: not BrandColor.None and < BrandColor.Rainbow } && this.HasEnoughEnergy()
               || this.CanonicalKeywords.Contains(HatMagician2Keywords.Evoke);
    }

    // 是否有足够的绘色
    public bool HasEnoughEnergy()
    {
        return BrandColorEnergyMgr.HasEnoughEnergy(this.Owner, this.BaseBrandColor, this.GetBrandColorCostWithModifiers());
    }

    // 打出时尝试消耗绘色
    public void SpendEnergy()
    {
        if (!this.HasEnoughEnergy())
        {
            // 下次打出不能触发额外效果 反过来写的原因是免费打出时不会走到这一步
            this.NextCannotCost = true;
            return;
        }

        BrandColorEnergyState state = BrandColorEnergyMgr.Instance.GetState(this.Owner);
        state.SpendEnergy(this.BaseBrandColor, this.GetBrandColorCostWithModifiers());
    }

    // 完整效果
    protected virtual async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    // 绘色不足时的普通效果
    protected virtual async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!this.NextCannotCost)
            await this.OnPlayWhenCostBrandColor(choiceContext, cardPlay);
        else
            await this.OnPlayNormal(choiceContext, cardPlay);
        await base.OnPlay(choiceContext, cardPlay);
    }
}