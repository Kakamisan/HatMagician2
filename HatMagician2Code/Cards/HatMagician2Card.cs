using BaseLib.Abstracts;
using BaseLib.Extensions;
using HarmonyLib;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;
using HatMagician2.HatMagician2Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
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

    // 是否有不消耗绘色即可打出印记的效果
    public virtual bool HasFreeBrandApply => false;

    // 是否有打出印记的效果
    public virtual bool HasBrandApply => false;

    // 是否结束回合效果
    public virtual bool HasEndTurn => false;

    // 次要卡牌对象 用于无法打出绘色效果时替换原本的卡牌对象
    public virtual TargetType? SubTargetType => null;

    // 子类自己的Tips
    protected virtual IEnumerable<IHoverTip> Hat2ExtraHoverTips => [];

    // 子类自己的Vars
    protected virtual IEnumerable<DynamicVar> Hat2ExtraCanonicalVars => [];

    // 子类自己的Keyword
    protected virtual IEnumerable<CardKeyword> Hat2CanonicalKeywords => [];

    // 子类自己的Tag
    protected virtual HashSet<CardTag> Hat2CanonicalTags => [];

    // 变化后的绘色消耗
    public int BrandColorCost => this.DynamicBrandCost.IntValue;

    // 绘色消耗Var
    public BrandColorCostVar DynamicBrandCost => (BrandColorCostVar)this.GetDynamicVar(BrandColorCostVar.DefaultName);

    // 额外的一个通用Var
    public Hat2Var DynamicHat2Var => (Hat2Var)this.GetDynamicVar(Hat2Var.DefaultName);

    // 添加通用Tips
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var hasExtra = false;
            IEnumerable<IHoverTip> baseTips = [];

            if (this.HasBrandApply)
            {
                if (Hat2ModConfig.ShowBaseBrandColorTips)
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandPower>());
                hasExtra = true;
            }

            baseTips = this.BaseBrandColor switch
            {
                BrandColor.Red => baseTips.AddItem(HoverTipFactory.FromPower<BrandRedPower>()),
                BrandColor.Blue => baseTips.AddItem(HoverTipFactory.FromPower<BrandBluePower>()),
                BrandColor.Yellow => baseTips.AddItem(HoverTipFactory.FromPower<BrandYellowPower>()),
                BrandColor.Orange => baseTips.AddItem(HoverTipFactory.FromPower<BrandOrangePower>()),
                BrandColor.Purple => baseTips.AddItem(HoverTipFactory.FromPower<BrandPurplePower>()),
                BrandColor.White => baseTips.AddItem(HoverTipFactory.FromPower<BrandWhitePower>()),
                _ => baseTips
            };

            return (hasExtra ? baseTips : []).Concat(this.Hat2ExtraHoverTips);
        }
    }

    // 添加通用数值
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        ((IEnumerable<DynamicVar>)[new EvokePreviewVar(), new BrandColorCostVar(this.BaseBrandColorCost)]).Concat(this.Hat2ExtraCanonicalVars);

    // 添加通用关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => ((IEnumerable<CardKeyword>)[]).Concat(this.Hat2CanonicalKeywords);

    // 添加通用Tag
    protected override HashSet<CardTag> CanonicalTags
    {
        get
        {
            HashSet<CardTag> set = [];
            set.UnionWith(this.Hat2CanonicalTags);
            return set;
        }
    }

    public override TargetType TargetType
    {
        get
        {
            if (this.SubTargetType != null)
            {
                return this.HasEnoughEnergy() ? base.TargetType : (TargetType)this.SubTargetType;
            }

            return base.TargetType;
        }
    }

    // 获取经过能力等修改后的绘色消耗
    public int GetBrandColorCostWithModifiers()
    {
        if (this.HasBrandColorCostX)
        {
            BrandColorEnergyState? state = HatMagician2Mgr.Instance?.GetState(this.Owner);
            return state != null ? state.BrandColorEnergyMap[this.BaseBrandColor] : 0;
        }

        CardPile? pile = this.Pile;
        return pile is { IsCombatPile: true } && this.CombatState != null
            ? (int)HatMagician2Mgr.ModifyBrandColorCost(this.CombatState, this, this.BrandColorCost)
            : this.BrandColorCost;
    }

    // 战斗状态相关
    // 下次攻击伤害变为N倍 只在火焰印记被刻印 且刻印时打出的是攻击牌时设置此值
    // 打出后复原此值
    public decimal NextPlayMulti = 1;
    public void SetNextPlayMulti(decimal value) => this.NextPlayMulti = value;
    public bool IsBrandApplied; // 是否已应用印记效果（判断是否要触发火焰印记N倍伤害 用于预览计算伤害）
    public bool NextCannotCost; // 是否无法消耗绘色（消耗绘色可打出额外效果 无法消耗则不能打出）
    public bool IsSleepApplied; // 是否已触发睡衣
    public bool NeedDream; // 是否触发梦乡自动从抽牌堆打出

    // 火焰印记临时设置倍率后修改伤害值 打出后倍率复原
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return cardSource == this && MultiDamagePower.IsTriggerMulti(cardSource)
            ? this.NextPlayMulti
            : base.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
    }

    // 打出后倍率复原 临时状态重置
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
               || this is { BaseBrandColor: not BrandColor.None and < BrandColor.Rainbow, HasBrandApply: true } && this.HasEnoughEnergy()
               || this.CanonicalKeywords.Contains(HatMagician2Keywords.Evoke);
    }

    // 是否有足够的绘色
    public bool HasEnoughEnergy()
    {
        if (this.CombatState == null)
            return true;
        return HatMagician2Mgr.HasEnoughEnergy(this.Owner, this.BaseBrandColor, this.GetBrandColorCostWithModifiers());
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

        BrandColorEnergyState? state = HatMagician2Mgr.Instance?.GetState(this.Owner);
        state?.SpendEnergy(this.BaseBrandColor, this.GetBrandColorCostWithModifiers());
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

    // 处理睡衣效果 每回合首次手牌为空时 这张卡从弃牌/消耗堆加入手牌
    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != this.Owner || !IsValidPhase(player.PlayerCombatState))
            return;
        if (this.CanonicalKeywords.Contains(HatMagician2Keywords.Sleep) && !this.IsSleepApplied)
        {
            this.IsSleepApplied = true;
            if (this.Pile is { Type: PileType.Discard or PileType.Exhaust })
            {
                await CardPileCmd.Add(this, PileType.Hand);
            }
        }

        await base.AfterHandEmptied(choiceContext, player);
    }

    private static bool IsValidPhase(PlayerCombatState? state)
    {
        if (state == null)
            return false;
        var phase = state.Phase;
        bool flag = phase switch
        {
            PlayerTurnPhase.AutoPrePlay or PlayerTurnPhase.Play or PlayerTurnPhase.AutoPostPlay => true,
            _ => false
        };

        return flag;
    }

    // 回合结束后重置睡衣状态 梦乡状态
    public override Task AfterTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side)
    {
        this.IsSleepApplied = false;
        this.NeedDream = false;
        return base.AfterTurnEndLate(choiceContext, side);
    }

    // 处理梦乡效果 使用睡衣卡结束回合时 打出抽牌堆的梦乡卡
    public override Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this.Keywords.Contains(HatMagician2Keywords.Dream) && cardPlay.Card.Keywords.Contains(HatMagician2Keywords.Sleep) &&
            cardPlay.Card is HatMagician2Card { HasEndTurn: true })
        {
            this.NeedDream = true;
        }

        return base.AfterCardPlayedLate(choiceContext, cardPlay);
    }

    // 回合结束出牌阶段自动打出梦乡卡
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != this.Owner)
            return;
        var cardPile = this.Pile;
        if (cardPile is not { Type: PileType.Draw })
            return;
        if (!this.NeedDream)
            return;
        await CardCmd.AutoPlay(choiceContext, this, null);
    }
}