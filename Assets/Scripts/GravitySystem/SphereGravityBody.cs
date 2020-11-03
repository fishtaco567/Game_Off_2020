using System.Collections.Generic;
using UnityEngine;

class SphereGravityBody : GravityBody {

    // Use this for initialization
    protected override void Start() {
        base.Start();
    }
    
    protected override void FixedUpdate() {
    }

    public override Vector3 NormalFor(Vector3 position) {
        return (position - transform.position).normalized;
    }

    public override float DistanceFrom(Vector3 position) {
        return (position - transform.position).magnitude;
    }

    public override Vector3 RandomPointAbove(float dist) {
        return Random.onUnitSphere * dist + transform.position;
    }

}
