using System;
using System.Collections.Generic;
using UnityEngine;


namespace CMPM.Movement {
    public class Unit : MonoBehaviour {
        public Vector2 movement;
        public float speed;

        void FixedUpdate() {
            movement = movement.normalized * speed;
            Move(new Vector2(movement.x, 0) * Time.fixedDeltaTime);
            Move(new Vector2(0, movement.y) * Time.fixedDeltaTime);
        }

        public void Move(Vector2 ds) {
            List<RaycastHit2D> hits = new();
            int                n    = GetComponent<Rigidbody2D>().Cast(ds, hits, ds.magnitude * 2);
            if (n == 0) transform.Translate(ds);
        }
    }
}