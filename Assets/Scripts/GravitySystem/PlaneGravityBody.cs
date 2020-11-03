using System.Collections.Generic;
using UnityEngine;

class PlaneGravityBody : GravityBody {

    public Vector2 size;
    public Vector2 offset;

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
        return transform.up;
    }

    public override float DistanceFrom(Vector3 position) {
        position = position - transform.position;
        var projectedPos = Vector3.ProjectOnPlane(position, transform.up);
        return (projectedPos - position).magnitude;
    }

    public override Vector3 RandomPointAbove(float dist) {
        Vector3 pointOnPlane = new Vector3(Random.Range(-size.x + offset.x, size.x + offset.x), 0, Random.Range(-size.y + offset.y, size.y + offset.y));
        return pointOnPlane + transform.up * dist;
    }

}