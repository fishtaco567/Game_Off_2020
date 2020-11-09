using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : FallBody {
	
    //Particle Systems
	[SerializeField]
	ParticleSystem runParticleSystem;
	[SerializeField]
	ParticleSystem jumpParticleSystem;
    
    ParticleSystem.EmissionModule runParticleEmission;
    

	[SerializeField]
	float moveForce = 6;

    [SerializeField]
	float jumpForce = 20f;

    [SerializeField]
    float maxSpeed = 15;

    [SerializeField]
    float lowSpeed = 5;

    [SerializeField]
    float speedupMod = 2;

    [SerializeField]
    float jumpMod = 5;

    [SerializeField]
    float stoppingForce = 5;

    [SerializeField]
    float sliderReductionForce = 2.5f;

    [SerializeField]
    float hoverForce;

    [SerializeField]
    float takeOffDelay;

    [SerializeField]
    AnimationCurve takeOffCurve;

    [SerializeField]
    float hoverTime;

    [SerializeField]
    float interactRadius = 6f;

    [SerializeField]
    Transform cam;

    [SerializeField]
    AudioClip footstepsSFX;

    [SerializeField]
    AudioClip jumpSFX;

    [SerializeField]
    AudioClip landSFX;

    [SerializeField]
    AudioClip collectSFX;

    [SerializeField]
    Animator animator;

    [SerializeField]
    float jumpTolerance;

    float jumpRaycastLength;

    bool onTheGround;

    bool lostControl;
	float lostControlTime;

    float currentHoverTime;

    [SerializeField]
    float currentTakeoffWaitTime;
    [SerializeField]
    float currentTakeoffTime;

    [SerializeField]
    float takeoffCurveTime;

    [SerializeField]
    bool takingOff;

    //TODO Better Jump
    [SerializeField]
    bool jumped;

    bool onSlope = false;
    Vector3 towardsSlope;

	Vector3 faceDirection;

    AudioSource footstepsAudioSource;

    AudioSource jumpAudioSource;

    AudioSource landAudioSource;

    AudioSource collectAudioSource;

    private Player playerInput;

    private PlayerAbility ability;

    private int numUsedJumps;

    //Input
    private bool jumpPressed;
    private bool jumpHeld;
    private bool takeoffHeld;
    private float horizontalIn;
    private float verticalIn;

    bool grounded;

    public GameObject RaycastDown {
        get {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -normal, out hit)) {
                return hit.collider.gameObject;
            }
            return null;
        }
    }
    
    //Returns the distance from the nearest body IN IT'S OWN SPACE
    public float DistanceFromBody {
        get {
            return Vector3.Distance(nearestBody.transform.position, transform.position);
        }
    }

    protected override void Start()
	{
		base.Start();

		if (cam == null) {
			cam = Camera.main.transform;
		}
        
		runParticleEmission = runParticleSystem.emission;

        footstepsAudioSource = gameObject.AddComponent<AudioSource>();
        footstepsAudioSource.loop = true;
        footstepsAudioSource.clip = footstepsSFX;

        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.clip = jumpSFX;

        landAudioSource = gameObject.AddComponent<AudioSource>();
        landAudioSource.clip = landSFX;

        collectAudioSource = gameObject.AddComponent<AudioSource>();
        collectAudioSource.clip = collectSFX;

        //Get the player
        playerInput = ReInput.players.GetPlayer(0);

        //Calculate jump tolerance assuming capsule collider
        jumpRaycastLength = GetComponent<CapsuleCollider>().height / 2 + jumpTolerance;

        ability = GetComponent<PlayerAbility>();

        currentHoverTime = 0;

        currentTakeoffWaitTime = 0;
        currentTakeoffTime = 0;
        takeoffCurveTime = takeOffCurve.keys[takeOffCurve.length - 1].time;

        takingOff = false;

        lostControl = false;
        lostControlTime = -1;
    }

    // Update is called once per frame
    protected override void Update() {
        Vector3 tangentVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, base.normal);
        animator.SetFloat("Velocity", Vector3.Magnitude(tangentVelocity));
    }

    protected override void FixedUpdate() {
        jumpPressed = playerInput.GetButtonDown("Jump");
        verticalIn = playerInput.GetAxis("Vert");
        horizontalIn = playerInput.GetAxis("Horiz");

        jumpHeld = playerInput.GetButton("Jump");
        takeoffHeld = playerInput.GetButton("BlastOff");

        base.FixedUpdate();

        if(nearestBody != null) {
            normal = nearestBody.NormalFor(transform.position);

            //Add extra force when falling
            if (onTheGround && !onSlope) {
                rigidbody.AddForce(-nearestBody.Magnitude * normal, ForceMode.Acceleration);
            } else {
                rigidbody.AddForce(-nearestBody.Magnitude * normal * jumpMod, ForceMode.Acceleration);
            }
        }

        //Lose control when damaged
        if (lostControl) {
            lostControlTime -= Time.fixedDeltaTime;
            if (lostControlTime < 0) {
                lostControl = false;
                //animator.SetTrigger("Recover");
            } else {
                jumpPressed = false;
                verticalIn = 0f;
                horizontalIn = 0f;
            }
        }

        grounded = CheckJump();

        animator.SetBool("Grounded", grounded);

        if ((!jumped || (numUsedJumps == 0 && ability.hasBumpNozzle)) && jumpPressed) {
            if(!grounded) {
                numUsedJumps++;
            }

            jumpAudioSource.Play();
            footstepsAudioSource.Stop();
            onTheGround = false;
            jumped = true;
            animator.SetTrigger("Jump");
            jumpParticleSystem.Emit(15);

            rigidbody.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if(jumpHeld && currentHoverTime < hoverTime) {
            currentHoverTime += Time.fixedDeltaTime;
            rigidbody.AddRelativeForce(Vector3.up * hoverForce, ForceMode.Force);
        }

        if(!takingOff && grounded && takeoffHeld) {
            if(currentTakeoffWaitTime < takeOffDelay) {
                currentTakeoffWaitTime += Time.fixedDeltaTime;
                //TODO animation and camera
            } else {
                takingOff = true;
            }
        } else if(!takingOff) {
            currentTakeoffWaitTime = 0;
        }

        animator.SetBool("Take Off", takeoffHeld);

        if(takingOff) {
            if(currentTakeoffTime > takeoffCurveTime) {
                takingOff = false;
                currentTakeoffTime = 0;
                currentTakeoffWaitTime = 0;
            }

            rigidbody.AddRelativeForce(Vector3.up * takeOffCurve.Evaluate(currentTakeoffTime) * ability.numFuel, ForceMode.Force);
            currentTakeoffTime += Time.fixedDeltaTime;
        }

        //Move toward 
        faceDirection = (
            Vector3.ProjectOnPlane(cam.forward, normal).normalized * verticalIn + Vector3.ProjectOnPlane(cam.right, normal).normalized * horizontalIn
        );

        //Tangent to the point on the surface of the planet... closeish
        Vector3 tangentVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, normal);
        float tangentSpeed = Vector3.Magnitude(tangentVelocity);

        //Speedup when the player hasn't began moving yet
        float speedup = 1;
        if (tangentSpeed < lowSpeed) {
            speedup = Mathf.Lerp(1, speedupMod, (lowSpeed - tangentSpeed) / lowSpeed);
        }

        //Main move force
        if (onSlope) {
            var curForce = transform.TransformVector(Vector3.forward * faceDirection.magnitude * moveForce * speedup);
            var forceTowardSlope = Vector3.Project(curForce, towardsSlope);
            if (Vector3.Dot(curForce, towardsSlope) < 0) {
                curForce -= forceTowardSlope;
            }
            rigidbody.AddForce(curForce, ForceMode.Force);
        } else {
            rigidbody.AddRelativeForce(Vector3.forward * faceDirection.magnitude * moveForce * speedup, ForceMode.Force);
        }

        // turn the run particles on if on the ground and (control) moving
        runParticleEmission.enabled = (
            onTheGround && faceDirection != Vector3.zero && !onSlope
        );

        //Set face direction to transform forward to preserve rotation
        if (faceDirection == Vector3.zero || onSlope) {
            faceDirection = Vector3.ProjectOnPlane(transform.forward, base.normal);
            footstepsAudioSource.Stop();
        } else if (!footstepsAudioSource.isPlaying && !jumped && !onSlope) {
            footstepsAudioSource.Play();
        }

        if (!lostControl) {
            //Slow down fast when not moving
            if ((Mathf.Abs(verticalIn) < 0.1 && Mathf.Abs(horizontalIn) < 0.1 && tangentSpeed > 0) && !onSlope) {
                var velChange = stoppingForce * -tangentVelocity.normalized;
                if (velChange.magnitude > tangentSpeed) {
                    velChange = velChange.normalized * tangentSpeed;
                }
                rigidbody.AddForce(velChange, ForceMode.VelocityChange);
            }

            //Preserve momentum and reduce sliding on corners
            Vector3 forceAgainst = tangentVelocity - Vector3.Project(tangentVelocity, faceDirection);
            if (Vector3.Magnitude(forceAgainst) > 0 && tangentSpeed > 0 && !onSlope) {
                if (forceAgainst.magnitude > tangentSpeed) {
                    forceAgainst = forceAgainst.normalized * tangentSpeed;
                }
                rigidbody.AddForce(-forceAgainst, ForceMode.VelocityChange);
                rigidbody.AddForce(faceDirection.normalized * forceAgainst.magnitude, ForceMode.VelocityChange);
            }
        } else {
            rigidbody.drag = .1f;
        }
        
        //Limit speed
        if (tangentSpeed > maxSpeed) {
            float brakeSpeed = tangentSpeed - maxSpeed;
            Vector3 brakeVel = tangentVelocity.normalized * brakeSpeed;

            rigidbody.AddForce(-brakeVel, ForceMode.VelocityChange);
        }

        //Rotate to face forward, normal to the planet
        rigidbody.MoveRotation(Quaternion.LookRotation(faceDirection, base.normal));

        animator.SetFloat("YSpeed", transform.InverseTransformVector(rigidbody.velocity).y);
    }

    private bool CheckJump() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, jumpRaycastLength, LayerMask.GetMask("Terrain"))) {
            return true;
        }

        return false;
    }

    public void TimeOut(float howLong = 2f) {
		//animator.SetTrigger("Hit");

		lostControlTime = howLong;
		lostControl = true;
	}

    public void OnCollect(CollectibleBase collectible) {
    
    }

    private void OnTriggerEnter(Collider other) {

    }

    private void OnCollisionStay(Collision collision) {
        onTheGround = true;
        jumped = false;
        numUsedJumps = 0;
        currentHoverTime = 0;

        var os = false;
        foreach (ContactPoint c in collision.contacts) {
            if (c.thisCollider.GetType() == typeof(CapsuleCollider)) {
                os = true;
                towardsSlope = Vector3.ProjectOnPlane(c.normal, normal).normalized;
            }
        }
        onSlope = os;
    }

    private void OnCollisionExit(Collision collision) {
        onTheGround = false;
        onSlope = false;
    }

}
