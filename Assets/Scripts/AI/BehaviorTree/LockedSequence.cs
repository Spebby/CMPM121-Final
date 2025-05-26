using System;
using System.Collections.Generic;


namespace CMPM.AI.BehaviourTree {
    public class LockedSequence : InteriorNode {
        public LockedSequence(IEnumerable<BehaviourTree> children) : base(children) { }
        bool _locked;

        public void Lock() {
            _locked = true;
        }

        public override Result Run() {
            if (_locked) return Result.FAILURE;
            if (CurrentChild >= Children.Count) CurrentChild = 0;

            Result res = Children[CurrentChild].Run();
            switch (res) {
                case Result.FAILURE:
                    if (CurrentChild == 0) _locked = true;
                    CurrentChild = 0;
                    return Result.IN_PROGRESS;
                case Result.SUCCESS:
                    CurrentChild++;
                    break;
                case Result.IN_PROGRESS:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Result.IN_PROGRESS;
        }

        public override BehaviourTree Copy() {
            return new LockedSequence(CopyChildren());
        }
    }
}