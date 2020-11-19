using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravityBody : MonoBehaviour {
    
	[SerializeField]
	protected float magnitude = 14f;

	[SerializeField]
	[Range(0,8)]
	protected float pushDistance = 0f;
    
	public float PushDistance {
		get { return pushDistance; }
	}
    
	protected float farthestCenter;
	public virtual float FarthestEdge {
		get { return farthestCenter + pushDistance + 2f; }
	}

    [SerializeField]
	protected bool expanding;

	[SerializeField]
	protected float expandRate = 0.003f;

	public float Magnitude {
		get { return magnitude; }
	}

	// Use this for initialization
	protected virtual void Start () {
		PlanetManager.Instance.RegisterBody(this);
		Debug.Log(transform.position + ", " + gameObject.name);
	}
	
	// Update is called once per frame
	protected virtual void FixedUpdate () {

    }

    public abstract Vector3 NormalFor(Vector3 position);

    public abstract float DistanceFrom(Vector3 position);

    public abstract Vector3 RandomPointAbove(float dist);

}
