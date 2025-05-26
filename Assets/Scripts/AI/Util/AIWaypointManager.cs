using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CMPM.AI.Util {
    public class AIWaypointManager {
        readonly List<AIWaypoint> _waypoints;

        static AIWaypointManager _theInstance;

        public static AIWaypointManager Instance {
            get { return _theInstance ??= new AIWaypointManager(); }
        }

        AIWaypointManager() {
            _waypoints = new List<AIWaypoint>();
        }

        public void AddWaypoint(AIWaypoint wp) {
            _waypoints.Add(wp);
        }

        public AIWaypoint GetClosest(Vector3 point) {
            return _waypoints.Aggregate((a, b) => (a.position - point).sqrMagnitude < (b.position - point).sqrMagnitude
                                            ? a
                                            : b);
        }

        public AIWaypoint GetClosestByType(Vector3 point, AIWaypoint.Type type) {
            List<AIWaypoint> ofType = _waypoints.FindAll((a) => a.type == type);
            if (ofType.Count == 0) return null;
            return ofType.Aggregate(
                (a, b) => (a.position - point).sqrMagnitude < (b.position - point).sqrMagnitude ? a : b);
        }

        public AIWaypoint Get(int i) {
            return _waypoints.Count <= i ? null : _waypoints[i];
        }
    }
}