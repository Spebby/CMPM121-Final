using System;
using CMPM.AI.BehaviourTree;
using CMPM.AI.BehaviourTree.Actions;
using CMPM.AI.BehaviourTree.Queries;
using CMPM.Enemies;
using CMPM.Movement;
using CMPM.Utils.AIParsers;
using Newtonsoft.Json;


namespace CMPM.AI {
    [JsonConverter(typeof(AIBehaviourTypeParser))]
    public enum BehaviourType {
        Support,
        Swarmer
    }
    
    public class BehaviourBuilder {
        public static BehaviourTree.BehaviourTree MakeTree(EnemyController agent) {
            BehaviourTree.BehaviourTree result = agent.type switch {
                BehaviourType.Support => new Selector(new BehaviourTree.BehaviourTree[] {
                    //Here we create the Warlock behavior tree

                    //Here we implement THE GROUPING UP PHASE
                    new LockedSequence(new BehaviourTree.BehaviourTree[] {
                        //If we're close enough, this fails, and so does the sequence. This is our "lock".
                        new NearbyEnemiesQueryReversed(3, 50f),
                        new Flee(25f),
                        new GoToClosestSupport(1.5f)
                    }),
                    new Loop(new BehaviourTree.BehaviourTree[] {
                        new PermaBuffIfPossible(),
                        new BuffIfPossible(),
                        new HealIfPossible(),
                        new MoveToPlayer(agent.GetAction(EnemyActionTypes.Attack).Range),
                        new Attack()
                    })

                    //Here we implement THE ATTACK PHASE
                }),

                BehaviourType.Swarmer => new Selector(new BehaviourTree.BehaviourTree[] {
                    //Here we create the Grouping behavior tree
                    new LockedSequence(new BehaviourTree.BehaviourTree[] {
                        new NearbyEnemiesQueryReversed(3, 50f),
                        new Flee(25f),
                        new GoToClosestSwarmer(1.5f)
                    }),
                    new Sequence(new BehaviourTree.BehaviourTree[] {
                        new MoveToPlayer(agent.GetAction(EnemyActionTypes.Attack).Range),
                        new Attack()
                    })
                }),

                _ => throw new NotImplementedException($"{agent.type} is not supported!")
            };

            // do not change/remove: each node should be given a reference to the agent
            foreach (BehaviourTree.BehaviourTree n in result.AllNodes()) {
                n.SetAgent(agent);
            }

            return result;
        }
    }
}