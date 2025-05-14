using CMPM.AI.BehaviorTree;
using CMPM.AI.BehaviorTree.Actions;
using CMPM.AI.BehaviorTree.Queries;
using CMPM.Movement;


namespace CMPM.AI {
    public class BehaviorBuilder {
        public static BehaviorTree.BehaviorTree MakeTree(EnemyController agent) {
            BehaviorTree.BehaviorTree result = agent.monster switch {
                "warlock" => new Selector(new BehaviorTree.BehaviorTree[] {
                    //Here we create the Warlock behavior tree

                    //Here we implement THE GROUPING UP PHASE
                    new LockedSequence(new BehaviorTree.BehaviorTree[] {
                        new NearbyEnemiesQueryReversed(
                            5, 8f), //If we're close enough, this fails, and so does the sequence. This is our "lock".
                        new Flee(25f),
                        new GoToClosestWarlock(1.5f)
                    }),
                    new Loop(new BehaviorTree.BehaviorTree[] {
                        new PermaBuffIfPossible(),
                        new BuffIfPossible(),
                        new HealIfPossible(),
                        new MoveToPlayer(agent.GetAction("attack").Range),
                        new Attack()
                    })

                    //Here we implement THE ATTACK PHASE
                }),
                "zombie" => new Selector(new BehaviorTree.BehaviorTree[] {
                    //Here we create the Zombie behavior tree
                    new LockedSequence(new BehaviorTree.BehaviorTree[] {
                        new NearbyEnemiesQueryReversed(
                            5, 8f), //If we're close enough, this fails, and so does the sequence. This is our "lock".
                        new Flee(25f),
                        new GoToClosestBasic(1.5f)
                    }),
                    new Sequence(new BehaviorTree.BehaviorTree[] {
                        new MoveToPlayer(agent.GetAction("attack").Range),
                        new Attack()
                    })
                }),
                "skeleton" => new Selector(new BehaviorTree.BehaviorTree[] {
                    //Here we create the Skeleton behavior tree
                    new LockedSequence(new BehaviorTree.BehaviorTree[] {
                        new NearbyEnemiesQueryReversed(
                            5, 8f), //If we're close enough, this fails, and so does the sequence. This is our "lock".
                        new Flee(25f),
                        new GoToClosestBasic(1.5f)
                    }),
                    new Sequence(new BehaviorTree.BehaviorTree[] {
                        new MoveToPlayer(agent.GetAction("attack").Range),
                        new Attack()
                    })
                }),

                //Here we create the deafult behavior tree. Technically, I could make this the skeleton, but that doesn't sit right with me so I will be switching it up for readability.
                _ => new Sequence(new BehaviorTree.BehaviorTree[]
                                      { new MoveToPlayer(agent.GetAction("attack").Range), new Attack() })
            };

            // do not change/remove: each node should be given a reference to the agent
            foreach (BehaviorTree.BehaviorTree n in result.AllNodes()) {
                n.SetAgent(agent);
            }

            return result;
        }
    }
}