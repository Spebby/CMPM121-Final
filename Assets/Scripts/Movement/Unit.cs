using System;
using System.Collections.Generic;
using UnityEngine;


public class Unit : MonoBehaviour {
    public Vector2 movement;
    public float distance;
    public event Action<float> OnMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void FixedUpdate() {
        this.Move(new Vector2(movement.x, 0) * Time.fixedDeltaTime);
        this.Move(new Vector2(0, movement.y) * Time.fixedDeltaTime);
        distance += movement.magnitude * Time.fixedDeltaTime;
        if (!(distance > 0.5f)) return;
        this.OnMove?.Invoke(distance);
        distance = 0;
    }

    public void Move(Vector2 ds) {
        var hits = new List<RaycastHit2D>();
        int n    = this.GetComponent<Rigidbody2D>().Cast(ds, hits, ds.magnitude * 2);
        if (n != 0) return;
        transform.Translate(ds);
    }
}