using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace HatMagician2.HatMagician2Code.Intents;

public class ColorfulIntent : CardDebuffIntent
{
    protected override string IntentPrefix => "HATMAGICIAN2-COLORFUL";
    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner) => this._cachedAnimationName ??= "CARD_DEBUFF".ToLowerInvariant();
}