using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HatMagician2.HatMagician2Code.Character;
using HatMagician2.HatMagician2Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace HatMagician2.HatMagician2Code.Cards;

[Pool(typeof(HatMagician2CardPool))]
public abstract class HatMagician2Card(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath =>
        (IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath =>
        (IsTest ? "card.png" : $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    public override string BetaPortraitPath =>
        (IsTest ? "card.png" : $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png").CardImagePath();

    // 设置成test的话使用通用的测试卡图
    protected bool IsTest = false;
}