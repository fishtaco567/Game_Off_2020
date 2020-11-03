using System.Collections.Generic;
using UnityEngine;

class TubeGravityBody : GravityBody {

    public float length;
    public float offset;

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
        position = position - transform.position;
        var projectedPos = Vector3.Project(position, transform.forward);
        return (position - projectedPos).normalized;
    }

    public override float DistanceFrom(Vector3 position) {
        position = position - transform.position;
        var projectedPos = Vector3.Project(position, transform.forward);
        return (projectedPos - position).magnitude;
    }

    public override Vector3 RandomPointAbove(float dist) {
        var pointAlongLine = transform.position + transform.forward * (Random.Range(-length + offset, length + offset));
        var angle = Random.Range(0, 2 * Mathf.PI);
        var vector = transform.TransformVector(new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle)));
        return pointAlongLine + vector * dist;
    }

}