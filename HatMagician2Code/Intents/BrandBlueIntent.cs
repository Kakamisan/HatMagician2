using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace HatMagician2.HatMagician2Code.Intents;

public class BrandBlueIntent : DebuffIntent
{
    protected override string IntentPrefix => "HATMAGICIAN2-BRAND_BLUE";
    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner) => this._cachedAnimationName ??= "DEBUFF".ToLowerInvariant();
    protected override string SpritePath => "intents/brand_blue.png";
}