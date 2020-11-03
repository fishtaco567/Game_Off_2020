using System.Collections.Generic;
using UnityEngine;

class TorusGravityBody : GravityBody {

    public float radius;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        //TODO setup expansion
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        //TODO expand
    }

    public override Vector3 NormalFor(Vector3 position) {
        return (position - RootFor(position)).normalized;
    }

    public override float DistanceFrom(Vector3 position) {
        Debug.DrawLine(position, RootFor(position), Color.blue, 5);
        return Vector3.Magnitude(position - RootFor(position));
    }

    public override Vector3 RandomPointAbove(float dist) {
        return transform.up * dist;
    }

    public Vector3 RootFor(Vector3 position) {
        Vector3 direction = Vector3.Normalize((position - transform.position) - Vector3.Project(position, transform.up));
        return direction * radius + transform.position;
    }

}