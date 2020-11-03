using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallBody : MonoBehaviour {
	public GravityBody nearestBody;

	new protected Rigidbody rigidbody;

	public Vector3 normal;

	[SerializeField]
	bool undying;

	[SerializeField]
	float specificGravityMultiplier = 1f;

	[SerializeField]
	protected float smoothingLerp = 12f;

	protected Transform homeParent;
	public Transform Home {
		get { return homeParent; }
	}

	// Use this for initialization
	protected virtual void Start () {
		if (nearestBody == null) {
			// If we have no gravity body set attach to the first found.
			nearestBody = FindObjectOfType<GravityBody>();
		}	
		// guess that we're right side up at the start. 
		normal = nearestBody.NormalFor(transform.position);

		rigidbody = GetComponent<Rigidbody>();

		homeParent = transform.parent;
	}

	protected virtual void FixedUpdate () {
		Vector3 surfaceNormal = nearestBody.NormalFor(transform.position);

		normal = Vector3.Lerp(normal, surfaceNormal, smoothingLerp * Time.deltaTime);

		rigidbody.AddForce(-nearestBody.Magnitude * surfaceNormal * rigidbody.mass * specificGravityMultiplier);

		var frontFace = Vector3.Cross(transform.right, normal);

		var targetRotation = Quaternion.LookRotation(frontFace, normal);

		this.rigidbody.MoveRotation(targetRotation);
	}

	// Update is called once per frame
	protected virtual void Update () {
	}

    public virtual void Die() {
        Destroy(gameObject);
    }

	public void StartFloating () {
		specificGravityMultiplier = 0f;
	}

	public void StopFloating () {
		specificGravityMultiplier = 1f;
	}

	public void ReleaseToHome () {
		transform.SetParent(homeParent);
	}
}
