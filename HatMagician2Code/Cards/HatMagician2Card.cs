using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
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
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HatMagician2.HatMagician2Code.Cards;

public abstract class HatMagician2Card(int cost, CardType type, CardRarity rarity, TargetType target) : CustomCardModel(cost, type, rarity, target), IHatMagician2AbstractModel
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of full art: 250x350
    //Smaller variant of normal art: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    // 绘色消耗类型
    public virtual BrandColor BaseBrandColor => BrandColor.None;

    // 绘色X费
    public virtual bool HasBrandColorCostX => false;

    // 基础绘色消耗
    public virtual int BaseBrandColorCost => -1;

    // 是否有不消耗绘色即可打出印记的效果 (这里指对主要目标)
    public virtual bool HasFreeBrandApplyTarget => false;

    // 是否有打出印记的效果 (这里指对主要目标)
    public virtual bool HasBrandApplyTarget => this.HasFreeBrandApplyTarget;

    // 是否有打出印记的效果
    public virtual bool HasBrandApply => this.HasBrandApplyTarget;

    // 是否添加印记说明
    public virtual bool IsAddBrandTips => this.HasBrandApply;

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

    // 子类的是否可打出
    protected virtual bool IsPlayableSub => true;

    // 通用判断 不能从手牌打出
    protected override bool IsPlayable =>
        this.IsPlayableSub && (!this.Keywords.Contains(HatMagician2Keywords.OnlyDream) || this.Pile?.Type != PileType.Hand);

    // 添加通用Tips
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            IEnumerable<IHoverTip> baseTips = [];
            if (this.IsAddBrandTips)
            {
                if (Hat2ModConfig.ShowBaseBrandColorTips)
                    baseTips = baseTips.AddItem(HoverTipFactory.FromPower<BrandPower>());
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
            }

            return baseTips.Concat(this.Hat2ExtraHoverTips);
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
                // 无法使用绘色效果时 目标类型改成子类型
                return this.HasEnoughEnergy() || this.Pile?.Type != PileType.Hand ? base.TargetType : (TargetType)this.SubTargetType;
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

    protected LocString SelectionScreenPrompt2
    {
        get
        {
            LocString str = new LocString("cards", this.Id.Entry + ".selectionScreenPrompt2");
            if (!str.Exists())
                throw new InvalidOperationException($"No selection screen prompt 2 for {this.Id}.");
            this.DynamicVars.AddTo(str);
            return str;
        }
    }

    // 战斗状态相关
    // 下次攻击伤害变为N倍 只在火焰印记被刻印 且刻印时打出的是攻击牌时设置此值
    // 打出后复原此值
    public decimal NextPlayMulti = 1;
    public void SetNextPlayMulti(decimal value) => this.NextPlayMulti = value;
    public bool IsBrandApplied; // 是否已应用印记效果（判断是否要触发火焰印记N倍伤害 用于预览计算伤害）
    public virtual bool IsAoeAttack => false; // 是否Aoe攻击卡 （Aoe卡不需要计算预览火焰印记N倍伤害 由灼痕能力处理）
    public bool NextCannotCost; // 是否无法消耗绘色（消耗绘色可打出额外效果 无法消耗则不能打出）
    public bool IsSleepApplied; // 是否已触发睡衣
    public bool NeedDream; // 是否触发梦境自动从抽牌堆打出

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
            this._tmpBrandColorCosts.RemoveAll(c => c.ClearsWhenCardIsPlayed);
        }

        return base.AfterCardPlayed(choiceContext, cardPlay);
    }

    // 是否可触发刻印的卡
    public bool IsEvokeCard()
    {
        return this.HasFreeBrandApplyTarget
               || this is { BaseBrandColor: not BrandColor.None and < BrandColor.Rainbow, HasBrandApplyTarget: true } && this.HasEnoughEnergy()
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
    public async Task SpendEnergy()
    {
        if (!this.HasEnoughEnergy())
        {
            // 下次打出不能触发额外效果 反过来写的原因是免费打出时不会走到这一步
            this.NextCannotCost = true;
            return;
        }

        BrandColorEnergyState? state = HatMagician2Mgr.Instance?.GetState(this.Owner);
        if (state != null)
        {
            var cost = this.GetBrandColorCostWithModifiers();
            await state.SpendEnergy(this.BaseBrandColor, cost);
            this.LastBrandColorCost = cost;
        }
    }

    // 绘色充足时的完整效果
    protected virtual async Task OnPlayWhenCostBrandColor(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    // 绘色不足时的普通效果 一般先执行绘色效果再执行普通效果 也可以分开两个单独写
    protected virtual async Task OnPlayNormal(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this.HasEndTurn) PlayerCmd.EndTurn(this.Owner, false);
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

    // 处理睡衣效果 每回合首次手牌为空时 这张卡从弃牌加入手牌
    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != this.Owner || !IsValidPhase(player.PlayerCombatState))
            return;
        if (this.CanonicalKeywords.Contains(HatMagician2Keywords.Sleep) && !this.IsSleepApplied)
        {
            this.IsSleepApplied = true;
            // if (this.Pile is { Type: PileType.Discard or PileType.Exhaust })
            if (this.Pile is { Type: PileType.Discard })
            {
                this.EnergyCost.AddThisTurn(DreamButterflyPower.AddCostThisTurn(this.Owner));
                this.AddBrandColorCostThisTurn = DreamButterflyPower.AddBrandColorCostThisTurn(this.Owner);
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

    // 回合结束后重置睡衣状态 梦境状态
    public override Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        this.IsSleepApplied = false;
        this.NeedDream = false;
        this._tmpBrandColorCosts.RemoveAll(c => c.ClearsWhenTurnEnds);
        this.ResetAddBrandColorCostThisTurn();
        return base.AfterSideTurnEndLate(choiceContext, side, participants);
    }

    // 处理梦境效果 使用睡衣卡结束回合时 打出抽牌堆的梦境卡
    // 处理浸入灵魂 所有印记牌添加侵蚀
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 打出结束回合的睡衣时间卡并且此卡为梦境卡 则设置成回合结束自动出牌
        if (this.Keywords.Contains(HatMagician2Keywords.Dream) && cardPlay.Card.Keywords.Contains(HatMagician2Keywords.Sleep) &&
            cardPlay.Card is HatMagician2Card { HasEndTurn: true })
        {
            this.NeedDream = true;
            // 现世与梦境的逆转在抽牌堆时 塞到最下面
            if (this is InversionOfRealityAndDream && this.Pile?.Type == PileType.Draw)
            {
                await CardPileCmd.Add([this], PileType.Draw, CardPilePosition.Bottom, null, true);
            }
        }

        // 打出浸入灵魂时若这张卡是印记卡 则附加侵蚀
        if (this.HasBrandApply && cardPlay.Card is SoulPermeation && !this.Keywords.Contains(HatMagician2Keywords.Erosion))
        {
            this.AddKeyword(HatMagician2Keywords.Erosion);
        }

        await base.AfterCardPlayedLate(choiceContext, cardPlay);
    }

    // 处理浸入灵魂 战斗中生成印记牌时添加侵蚀
    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (this.HasBrandApply && !this.Keywords.Contains(HatMagician2Keywords.Erosion) && card.Owner.HasPower<SoulPermeationPower>())
        {
            this.AddKeyword(HatMagician2Keywords.Erosion);
        }

        return base.AfterCardGeneratedForCombat(card, creator);
    }

    // 回合结束出牌阶段自动打出梦境卡
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

    // 绘色免费消耗相关
    private List<TemporaryCardCost> _tmpBrandColorCosts = [];
    private int _addBrandColorCostThisTurn;

    private int AddBrandColorCostThisTurn
    {
        get => this._addBrandColorCostThisTurn;
        set => this._addBrandColorCostThisTurn += value;
    }

    private void ResetAddBrandColorCostThisTurn() => this._addBrandColorCostThisTurn = 0;

    public bool TryModifyBrandColorCost(HatMagician2Card card, decimal originalCost, out decimal modifiedCost)
    {
        if (card == this && !this.HasBrandColorCostX)
        {
            var cost = this._tmpBrandColorCosts.FirstOrDefault();
            if (cost != null)
            {
                // 现在这个只有免费打出用到 所以不用比较哪个消耗更低 直接返回
                modifiedCost = Math.Min(cost.Cost, originalCost);
                return originalCost != modifiedCost;
            }

            modifiedCost = Math.Max(0, this.AddBrandColorCostThisTurn + originalCost);
            return originalCost != modifiedCost;
        }

        modifiedCost = originalCost;
        return false;
    }

    // patch 本回合绘色免费
    public void SetToFreeThisTurnForBrandColor()
    {
        this._tmpBrandColorCosts.Add(TemporaryCardCost.ThisTurn(0));
    }

    // patch 本场战斗绘色免费
    public void SetToFreeThisCombatForBrandColor()
    {
        this._tmpBrandColorCosts.Add(TemporaryCardCost.ThisCombat(0));
    }

    // public override Task BeforeCombatStartLate()
    // {
    //     //this._tmpBrandColorCosts.Clear();
    //     return base.BeforeCombatStartLate();
    // }

    // 深拷贝obj
    protected override void DeepCloneFields()
    {
        this._tmpBrandColorCosts = this._tmpBrandColorCosts.ToList();
        base.DeepCloneFields();
    }

    // 通用造成单体伤害调用
    protected async Task CommonSingleAttack(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target!).WithHitFx("vfx/vfx_starry_impact").Execute(choiceContext);
        await Task.CompletedTask;
    }

    protected async Task CommonSingleAttack(PlayerChoiceContext choiceContext, CardPlay play, int cnt)
    {
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).WithHitCount(cnt).Targeting(play.Target!).WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
        await Task.CompletedTask;
    }

    // 通用aoe伤害
    protected async Task CommonAoeAttack(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (this.CombatState == null) return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(this.CombatState).WithHitFx("vfx/vfx_starry_impact").Execute(choiceContext);
        await Task.CompletedTask;
    }

    protected async Task CommonAoeAttack(PlayerChoiceContext choiceContext, CardPlay play, int cnt)
    {
        if (this.CombatState == null) return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard(this).WithHitCount(cnt).TargetingAllOpponents(this.CombatState).WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
        await Task.CompletedTask;
    }

    // 通用应用倍数类能力 叠加时基础-1
    protected async Task CommonApplySelfMultiPower<T>(PlayerChoiceContext choiceContext, CardPlay play, decimal applyAmount) where T : PowerModel
    {
        if (this.Owner.Creature.HasPower<T>())
        {
            await PowerCmd.Apply<T>(choiceContext, this.Owner.Creature, applyAmount - 1, this.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<T>(choiceContext, this.Owner.Creature, applyAmount, this.Owner.Creature, this);
        }
    }

    // 通用应用常规能力
    protected async Task CommonApplySelfPower<T>(PlayerChoiceContext choiceContext, CardPlay play, decimal applyAmount) where T : PowerModel
    {
        await PowerCmd.Apply<T>(choiceContext, this.Owner.Creature, applyAmount, this.Owner.Creature, this);
    }

    // 通用Debuff
    protected async Task CommonApplyTargetPower<T>(PlayerChoiceContext choiceContext, CardPlay play, decimal amount) where T : PowerModel
    {
        await PowerCmd.Apply<T>(choiceContext, play.Target!, amount, this.Owner.Creature, this);
    }

    // 通用Aoe Debuff
    protected async Task CommonAoeApplyTargetPower<T>(PlayerChoiceContext choiceContext, decimal amount) where T : PowerModel
    {
        if (this.CombatState == null) return;
        foreach (var e in this.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<T>(choiceContext, e, amount, this.Owner.Creature, this);
        }
    }

    // 通用获得格挡
    protected async Task CommonBlock(CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
    }

    protected async Task CommonBlock(CardPlay play, decimal amount)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, amount, ValueProp.Move, play);
    }

    // X药处理
    public int LastBrandColorCost;

    public int ResolveBrandColorCostXValue()
    {
        if (!this.HasBrandColorCostX)
            throw new InvalidOperationException("This card does not have an X-cost.");
        if (this.CombatState == null) return 0;
        // 捕捉上次消耗绘色数量 应该是重放要用
        return Hook.ModifyXValue(this.CombatState, this, this.LastBrandColorCost);
    }
}