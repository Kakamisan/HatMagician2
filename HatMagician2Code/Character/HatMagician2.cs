using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using HatMagician2.HatMagician2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using HatMagician2.HatMagician2Code.Cards;
using HatMagician2.HatMagician2Code.Relics;

namespace HatMagician2.HatMagician2Code.Character;

public class HatMagician2 : PlaceholderCharacterModel
{
    public const string CharacterId = "HatMagician2";

    public static readonly Color Color = new("f4ebb2");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<BlankPainting>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<PaletteBottle>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<HatMagician2CardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HatMagician2RelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HatMagician2PotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    // 人物小头像路径
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    // 人物选择半身像
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    // 人物选择半身像-锁定状态
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    // 人物选择背景
    public override string CustomCharacterSelectBg => "select_bg.tscn".ScenePath();
    // 人物战斗模型
    public override string CustomVisualPath => "battle_character.tscn".ScenePath();
}