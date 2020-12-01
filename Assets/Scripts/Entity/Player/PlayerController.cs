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

    [SerializeField]
    ParticleSystem rocketSmokeParticleSystem;
    [SerializeField]
    ParticleSystem rocketFireParticleSystem;

    ParticleSystem.EmissionModule rocketSmokeEmission;
    ParticleSystem.EmissionModule rocketFireEmission;

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
    float knockbackForce;

    [SerializeField]
    float knockbackUp;

    [SerializeField]
    float hoverTime;

    [SerializeField]
    float interactRadius = 6f;

    [SerializeField]
    Transform cam;

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
    public float currentTakeoffWaitTime;
    [SerializeField]
    float currentTakeoffTime;

    [SerializeField]
    float takeoffCurveTime;

    [SerializeField]
    float jumpHoverDelay;

    [SerializeField]
    float maxSlope;

    [SerializeField]
    bool takingOff;

    //TODO Better Jump
    [SerializeField]
    bool jumped;

    [SerializeField]
    bool onSlope = false;
    [SerializeField]
    Vector3 towardsSlope;

	Vector3 faceDirection;

    [SerializeField]
    AudioSource footstepsAudioSource;

    [SerializeField]
    AudioSource jumpAudioSource;

    [SerializeField]
    AudioSource landAudioSource;

    [SerializeField]
    AudioSource collectAudioSource;

    [SerializeField]
    AudioSource hitAudioSource;

    [SerializeField]
    AudioSource hoverAudioSource;

    [SerializeField]
    AudioSource takeOffAudioSource;

    [SerializeField]
    AudioSource doubleJumpAudioSource;

    [SerializeField]
    AudioSource fuelAudioSource;

    private Player playerInput;

    private PlayerAbility ability;

    private int numUsedJumps;

    //Input
    private bool jumpPressed;
    private bool jumpHeld;
    public bool takeoffHeld;
    private float horizontalIn;
    private float verticalIn;
    private bool interactPressed;

    [SerializeField]
    bool grounded;

    private float timeSinceJumpPressed;

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

    float timeUngrounded;

    protected override void Start()
	{
		base.Start();
        touching = false;

        timeUngrounded = 0;

		if (cam == null) {
			cam = Camera.main.transform;
		}
        
		runParticleEmission = runParticleSystem.emission;
        rocketSmokeEmission = rocketSmokeParticleSystem.emission;
        rocketFireEmission = rocketFireParticleSystem.emission;

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
        timeSinceJumpPressed = 0;
    }

    // Update is called once per frame
    protected override void Update() {
        jumpRaycastLength = GetComponent<CapsuleCollider>().height / 2 + jumpTolerance;

        Vector3 tangentVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, base.normal);
        animator.SetFloat("Velocity", Vector3.Magnitude(tangentVelocity));
        animator.SetFloat("HitTimeout", lostControlTime);

        var unpaused = false;

        if(UIManager.Instance.inGameMenu.isOpen) {
            bool pausePressed = playerInput.GetButtonDown("Pause");
            if(pausePressed) {
                unpaused = true;
                UIManager.Instance.inGameMenu.ResumePressed();
            }
        }

        if(!UIManager.Instance.focused && !unpaused) {
            bool pausePressed = playerInput.GetButtonDown("Pause");
            if(pausePressed) {
                UIManager.Instance.inGameMenu.OpenInGameMenu();
            }
        }

        if(!UIManager.Instance.focused && !GameManager.Instance.isInMenu) {
            jumpPressed |= playerInput.GetButtonDown("Jump");
            verticalIn = playerInput.GetAxis("Vert");
            horizontalIn = playerInput.GetAxis("Horiz");

            jumpHeld = playerInput.GetButton("Jump");
            takeoffHeld = playerInput.GetButton("BlastOff");

            interactPressed = playerInput.GetButton("Interact");
        } else {
            jumpPressed = false;
            verticalIn = 0;
            horizontalIn = 0;
            jumpHeld = false;
            takeoffHeld = false;
            interactPressed = false;
        }

        if(interactPressed) {
            var colliders = Physics.OverlapSphere(transform.position, interactRadius, LayerMask.GetMask("Interact"));

            foreach(Collider col in colliders) {
                var npc = col.GetComponent<NPCController>();
                if(npc != null) {
                    npc.Interact(this);
                }
            }
        }

        animator.SetFloat("GroundProx", GetDistanceToGround(3f));
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        bool sliding = (onSlope && !grounded) || angle > maxSlope * Mathf.Rad2Deg;

        if(nearestBody != null) {
            normal = nearestBody.NormalFor(transform.position);

            //Add extra force when falling
            if(onTheGround && !onSlope) {
                rigidbody.AddForce(-nearestBody.Magnitude * normal, ForceMode.Acceleration);
            } else {
                rigidbody.AddForce(-nearestBody.Magnitude * normal * jumpMod, ForceMode.Acceleration);
            }
        }

        //Lose control when damaged
        if(lostControl) {
            lostControlTime -= Time.fixedDeltaTime;
            if(lostControlTime < 0) {
                lostControl = false;
                //animator.SetTrigger("Recover");
            } else {
            }
        }

        var wasGroundedLastFrame = grounded;

        grounded = CheckJump();

        if(!wasGroundedLastFrame && grounded && timeUngrounded > 0.3f) {
            cam.GetComponentInParent<CameraDolly>().Land();
            landAudioSource.Play();
        }

        animator.SetBool("Grounded", grounded);

        if(!grounded) {
            timeUngrounded += Time.deltaTime;
        } else {
            timeUngrounded = 0;
        }

        if(((!jumped && (grounded || timeUngrounded < 0.2)) || (numUsedJumps == 0 && ability.hasBumpNozzle)) && jumpPressed) {
            timeSinceJumpPressed = 0;

            if(!grounded && ability.hasBumpNozzle) {
                rocketSmokeParticleSystem.Emit(5);
                rocketFireParticleSystem.Emit(5);
                doubleJumpAudioSource.Play();
                numUsedJumps++;
            }

            jumpAudioSource.Play();
            footstepsAudioSource.Stop();
            onTheGround = false;
            jumped = true;
            animator.SetTrigger("Jump");
            jumpParticleSystem.Emit(15);

            rigidbody.AddRelativeForce(Vector3.up * jumpForce - new Vector3(0, transform.InverseTransformVector(rigidbody.velocity).y, 0), ForceMode.VelocityChange);
        }

        if(!grounded) {
            timeSinceJumpPressed += Time.fixedDeltaTime;
        }

        if(jumpHeld && currentHoverTime < hoverTime && timeSinceJumpPressed > jumpHoverDelay && ability.hasRestrictorNozzle && !onSlope && !touching) {
            rocketSmokeEmission.enabled = true;
            rocketFireEmission.enabled = true;
            currentHoverTime += Time.fixedDeltaTime;
            rigidbody.AddRelativeForce(Vector3.up * hoverForce, ForceMode.Force);
            if(!hoverAudioSource.isPlaying) {
                hoverAudioSource.Play();
            }
        } else {
            hoverAudioSource.Stop();
            rocketSmokeEmission.enabled = false;
            rocketFireEmission.enabled = false;
        }

        if(!takingOff && grounded && takeoffHeld) {
            if(currentTakeoffWaitTime < takeOffDelay) {
                currentTakeoffWaitTime += Time.fixedDeltaTime;
                //TODO camera
            } else {
                cam.GetComponentInParent<CameraDolly>().TakeOff();
                takingOff = true;
            }

            horizontalIn = 0;
            verticalIn = 0;
        } else if(!takingOff) {
            currentTakeoffWaitTime = 0;
        }

        if(cam.GetComponentInParent<CameraDolly>().isLookingUp) {
            horizontalIn = 0;
            verticalIn = 0;
        }

        animator.SetBool("Take Off", takeoffHeld);

        if(takingOff) {
            if(currentTakeoffTime > takeoffCurveTime) {
                takingOff = false;
                currentTakeoffTime = 0;
                currentTakeoffWaitTime = 0;
            }

            rocketSmokeEmission.enabled = true;
            rocketFireEmission.enabled = true;
            if(!takeOffAudioSource.isPlaying) {
                takeOffAudioSource.Play();
            }
            var rfMain = rocketFireParticleSystem.main;
            rfMain.startSize = 1.5f;
            rigidbody.AddRelativeForce(Vector3.up * takeOffCurve.Evaluate(currentTakeoffTime) * ability.numFuel, ForceMode.Force);
            currentTakeoffTime += Time.fixedDeltaTime;

            horizontalIn = 0;
            verticalIn = 0;
        } else {
            var rfMain = rocketFireParticleSystem.main;
            rfMain.startSize = 1;
            takeOffAudioSource.Stop();
        }

        if(sliding) {
            rigidbody.AddForce(towardsSlope * moveForce * 1.2f, ForceMode.Force);
            Debug.DrawLine(transform.position, transform.position + towardsSlope * 5f, Color.blue);
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
        if (onSlope || angle > maxSlope * Mathf.Rad2Deg) {
            var curForce = transform.TransformVector(Vector3.forward * faceDirection.magnitude * moveForce * speedup);
            var forceTowardSlope = Vector3.Project(curForce, towardsSlope);
            if (Vector3.Dot(curForce, towardsSlope) < 0) {
                curForce -= forceTowardSlope;
            }
            rigidbody.AddForce(curForce, ForceMode.Force);
        } 

        rigidbody.AddRelativeForce(Vector3.forward * faceDirection.magnitude * moveForce * speedup, ForceMode.Force);
        // turn the run particles on if on the ground and (control) moving
        runParticleEmission.enabled = (
            onTheGround && faceDirection != Vector3.zero && !onSlope
        );
        animator.SetBool("CloseToGround", runParticleEmission.enabled);

        //Set face direction to transform forward to preserve rotation
        if (faceDirection == Vector3.zero || onSlope) {
            faceDirection = Vector3.ProjectOnPlane(transform.forward, base.normal);
            footstepsAudioSource.Stop();
        } else if (!footstepsAudioSource.isPlaying && !jumped && !onSlope && grounded) {
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

        if(!takingOff && transform.InverseTransformVector(rigidbody.velocity).y < -75) {
            rigidbody.drag = .9f;
        } else {
            rigidbody.drag = 0.1f;
        }

        jumpPressed = false;
    }

    private bool CheckJump() {
        RaycastHit hit;

        float tanAngle = Mathf.Tan(maxSlope);

        if(Physics.Raycast(transform.position, -transform.up, out hit, jumpRaycastLength, LayerMask.GetMask("Terrain", "T2"))) {
            onTheGround = true;
            jumped = false;
            numUsedJumps = 0;
            currentHoverTime = 0;

            return true;
        }

        return false;
    }

    private float GetDistanceToGround(float maxDist) {
        RaycastHit hit;

        float tanAngle = Mathf.Tan(maxSlope);

        if(Physics.Raycast(transform.position - transform.up * 1, -transform.up, out hit, maxDist, LayerMask.GetMask("Terrain", "T2"))) {
            return hit.distance;
        }

        return maxDist;        
    }

    public void TimeOut(float howLong = 0.5f) {
		animator.SetTrigger("Hit");

		lostControlTime = howLong;
		lostControl = true;

        rigidbody.AddRelativeForce(Vector3.back * knockbackForce, ForceMode.VelocityChange);
        rigidbody.AddRelativeForce(Vector3.up * knockbackUp - new Vector3(0, transform.InverseTransformVector(rigidbody.velocity).y, 0), ForceMode.VelocityChange);

        hitAudioSource.Play();
	}

    public void OnCollect(CollectibleBase collectible) {
        if(collectible is MoneyCollectible) {
            collectAudioSource.Play();
        } else if(collectible is FuelCollectible) {
            fuelAudioSource.Play();
        }
    
    }

    private void OnTriggerEnter(Collider other) {

    }

    [SerializeField]
    float angle;

    bool touching;

    private void OnCollisionStay(Collision collision) {
        touching = true;
        var os = false;
        lostControl = false;
        lostControlTime = -1;
        foreach (ContactPoint c in collision.contacts) {
            if (c.thisCollider.GetType() == typeof(CapsuleCollider)) {
                os = true;
                towardsSlope = Vector3.ProjectOnPlane(c.normal, normal).normalized;
            }

            angle = Vector3.Angle(c.normal, normal);
        }
        onSlope = os;
    }

    private void OnCollisionExit(Collision collision) {
        onTheGround = false;
        onSlope = false;
        touching = false;
    }

    public override void OnSwitchBody() {
        base.OnSwitchBody();

        takingOff = false;
    }

}
