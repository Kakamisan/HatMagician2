using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;
using HatMagician2.HatMagician2Code.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public abstract class HatMagician2Card(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target), IHatMagician2AbstractModel
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath =>
        (IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of full art: 250x350
    //Smaller variant of normal art: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath =>
        (IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    public override string BetaPortraitPath =>
        (IsTest ? "card.png" : $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    // 设置成test的话使用通用的测试卡图
    protected bool IsTest = false;

    // 绘色消耗
    public int BaseBrandColorCost = 0;
    public int BrandColorCost => this.BaseBrandColorCost;

    // 绘色消耗类型
    public HatMagician2BrandColor BaseBrandColor = HatMagician2BrandColor.None;

    // 绘色X费
    public bool HasBrandColorCostX = false;
    
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
}