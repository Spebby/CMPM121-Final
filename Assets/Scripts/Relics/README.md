# About Relics

## Effect Types

|               Type                |                                                                           |
| :-------------------------------: | :------------------------------------------------------------------------ |
|             Gain Mana             | Restores the player's mana by some amount.                                |
|          Gain Spellpower          | Boosts the player's spellpower by some amount.                            |
|            Gain Health            | Restores the player's health by some amount.                              |
|         Gain Random Buff          | Boosts a random stat by some amount.                                      |
|   Modify Spell Cost Percentage    | Modifies the cost of every spell to be a factor of the base cost.         |
| Modifiy Spell Cooldown Percentage | Modifies the cooldown of every spell to be a factor of the base cooldown. |

**Gain Stat** is an effect that boosts the corresponding stat. Under the hood,
all of these boosts are essentially the same code, so this was consolidated into
a singular implementation. Each type simply maps to a different preset of the
same class. It should be noted that some effects, namely _Gain Health_ are
intended to use variables embedded into the RPN to have access to more nuanced
evaluations. In these cases, the variable table must be revised.

**Gain Random Buff** is an effect that boosts a random stat. All effects are
revertible, even gaining mana or health.

Gain Spell

## Precondition Types

|    Type     |                                                        |
| :---------: | :----------------------------------------------------- |
| Take Damage | When the player takes any damage.                      |
| Stand Still | When the player stands still for a set amount of time. |
|   On Kill   | When the player kills an enemy.                        |
|    Timer    | After a set amount of time. Repeats forever.           |
|    None     | There is no precondition!                              |

**Timer** has an associated property, `range`. A `range` is a pair of RPNStrings
representing the min and max range the timer can be. The amount time an effect
lasts will be within this range. If a set range is desired, simply set
`min == max`.

**None** is interesting as any effect is immediately applied upon gaining the
relic. The rationale here is that the type will be used on passive items, whose
effects are constant and irrevocable. _Rigor Mortis_ is a good example or a
passive relic.

## Expire Types

There are a couple expire types.

|    Type    |                                        |
| :--------: | :------------------------------------- |
|    Move    | Expires when the player moves.         |
| Cast Spell | Expires when a spell is casts.         |
|   Timer    | Expires after a range of time.         |
| Overwrite  | Expires when the effect is re-applied. |
|    None    | Never expires                          |

Most of these are pretty self evident. Though I wanted to clarify timer and
overwrite.

**Timer** is unlike its precondition counterpart in that it only fires once, and
does not repeat. It also shares a special interaction with the precondition
timer by triggering when the precondition fires. Of course, there is a delay
here.

**Overwrite** is intended to be paired with effects that can trigger in quick
succession. For each trigger of an effect, the next trigger will overwrite its
changes. This is useful for items that give random stat boosts--as previous
boosts will be removed when the next boost is applied. Another use case would be
to pair it with the `take-damage` precondition, where every time damage is
dealt, the previous buff is replaced. An example effect could gives be a
spellpower boost based on how much damage you just took.

## A note on Coroutines

What are classes for? To handle more complicated state. A lot of our relics do
not require complicated triggers. Take Damage, Kill an Enemy, these are all
handled perfectly well by delegates and lambdas. But we do need more complicated
behaviour, for the likes of Triggering on a timer. Coroutines are a good match
for this need, and we use them a fair bit. Out effects support a have a
"Standstill" precondition as well as timed triggers and expirations. The
underlying logic here is handled the same for most of these routines. When we
trigger the routine again, we most likely want to restart the timer. So, lets
just share this logic. All of this to say, I am writing this note due to some
spaghetti. Expire and Triggers both need coroutines, so I use a base class. But
I can't use a base class because every Expire and every Trigger _must_ inherit
from either `RelicTrigger` or `RelicExpire` respectively. C# does not support
inheriting from multiple base classes, so unfortunately, there's some
duplication going on. `RelicCoroutineTrigger` and `RelicCoroutineExpire` are
nearly identical in their code, they simply exist to allow their respective
children to inherit from the right super class. Please keep this in mind while
working with relics!
