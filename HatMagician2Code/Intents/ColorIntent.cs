using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace HatMagician2.HatMagician2Code.Intents;

public class ColorIntent : BuffIntent
{
    protected override string IntentPrefix => "HATMAGICIAN2-COLOR";
    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner) => this._cachedAnimationName ??= "BUFF".ToLowerInvariant();
}