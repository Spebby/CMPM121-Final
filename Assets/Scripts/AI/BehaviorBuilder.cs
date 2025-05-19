using System;
using CMPM.AI.BehaviorTree;
using CMPM.AI.BehaviorTree.Actions;
using CMPM.AI.BehaviorTree.Queries;
using CMPM.Enemies;
using CMPM.Movement;

namespace CMPM.AI {
    public enum BehaviorType {
        Support,
        Swarmer
    }
    
    public class BehaviorBuilder {
        public static BehaviorTree.BehaviorTree MakeTree(EnemyController agent) {
            BehaviorTree.BehaviorTree result = agent.type switch {
                BehaviorType.Support => new Selector(new BehaviorTree.BehaviorTree[] {
                    //Here we create the Warlock behavior tree

                    //Here we implement THE GROUPING UP PHASE
                    new LockedSequence(new BehaviorTree.BehaviorTree[] {
                        //If we're close enough, this fails, and so does the sequence. This is our "lock".
                        new NearbyEnemiesQueryReversed(5, 8f),
                        new Flee(25f),
                        new GoToClosestSupport(1.5f)
                    }),
                    new Loop(new BehaviorTree.BehaviorTree[] {
                        new PermaBuffIfPossible(),
                        new BuffIfPossible(),
                        new HealIfPossible(),
                        new MoveToPlayer(agent.GetAction(EnemyActionTypes.Attack).Range),
                        new Attack()
                    })

                    //Here we implement THE ATTACK PHASE
                }),

                BehaviorType.Swarmer => new Selector(new BehaviorTree.BehaviorTree[] {
                    //Here we create the Zombie behavior tree
                    new LockedSequence(new BehaviorTree.BehaviorTree[] {
                        new NearbyEnemiesQueryReversed(5, 8f),
                        new Flee(25f),
                        new GoToClosestSwarmer(1.5f)
                    }),
                    new Sequence(new BehaviorTree.BehaviorTree[] {
                        new MoveToPlayer(agent.GetAction(EnemyActionTypes.Attack).Range),
                        new Attack()
                    })
                }),

                _ => throw new NotImplementedException($"{agent.type} is not supported!")
            };

            // do not change/remove: each node should be given a reference to the agent
            foreach (BehaviorTree.BehaviorTree n in result.AllNodes()) {
                n.SetAgent(agent);
            }

            return result;
        }
    }
}