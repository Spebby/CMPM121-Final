using System;
using System.Collections.Generic;


namespace CMPM.AI.BehaviourTree {
    public class Loop : InteriorNode {
        public override Result Run() {
            if (CurrentChild >= Children.Count) CurrentChild = 0;

            Result res = Children[CurrentChild].Run();
            switch (res) {
                case Result.FAILURE:
                case Result.SUCCESS:
                    CurrentChild++;
                    break;
                case Result.IN_PROGRESS:
                    // Do nothing, just return InProgress
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Result.IN_PROGRESS;
        }

        public Loop(IEnumerable<BehaviourTree> children) : base(children) { }

        public override BehaviourTree Copy() {
            return new Loop(CopyChildren());
        }
    }
}