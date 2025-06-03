using System.Collections.Generic;
using UnityEngine;


namespace CMPM.Movement {
    public class Unit : MonoBehaviour {
        public Vector2 movement;
        public float speed;

        void FixedUpdate() {
            Move(new Vector2(movement.x, 0) * Time.fixedDeltaTime);
            Move(new Vector2(0, movement.y) * Time.fixedDeltaTime);
        }

        public void Move(Vector2 ds) {
            List<RaycastHit2D> hits = new();
            ContactFilter2D filter = new();
            filter.useTriggers = false;
            int                n    = GetComponent<Rigidbody2D>().Cast(ds, filter, hits, ds.magnitude * 2);
            if (n == 0) transform.Translate(ds);
        }
    }
}
