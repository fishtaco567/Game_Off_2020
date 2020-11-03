using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour {

	public PlayerController player;

    public GameObject mainCam;

    [SerializeField]
    float sensitivity = 15;

    [SerializeField]
    float maxFollowDist = 6.5f;

    [SerializeField]
    float minFollowDist = 4;

    [SerializeField]
    float minHeightFromPlayerFeet = 4;

    [SerializeField]
    float maxHeightFromPlayerFeet = 8;

    //Seconds
    public float centerTime = 1f;

    Quaternion targetRot;

    Quaternion mainCamTargetRot;

    float centerBehind;

    bool targeting;

    Vector3 baseOffset {
        get{ return new Vector3(0, maxHeightFromPlayerFeet, -(minFollowDist + maxFollowDist) / 2); }
    }

	// Use this for initialization
	void Start () {
        Vector3 playerPos = player.transform.position;
        Vector3 playerNormal = player.normal;

        transform.position = playerPos + player.transform.TransformVector(baseOffset);
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(playerPos - transform.position, playerNormal), playerNormal);
        mainCam.transform.rotation = Quaternion.LookRotation(playerPos - mainCam.transform.position, playerNormal);
    }


	// Update is called once per frame
	void LateUpdate () {
        //TODO allow freecam
        Vector3 playerPos = player.transform.position;
        Vector3 playerNormal = player.normal;

        float playerDistFromBody = player.DistanceFromBody;

        //Rotate the dolly to look at the player
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(playerPos - transform.position, playerNormal), playerNormal), Time.deltaTime * sensitivity * 3);
        var realRot = transform.rotation;
        var yFromBody = player.nearestBody.DistanceFrom(transform.position);

        //Camera follow

        //Follow behind player
        var curOffset = transform.InverseTransformVector(playerPos - transform.position);
        
        if (curOffset.z > maxFollowDist) {
            curOffset.z = maxFollowDist;
        } else if (curOffset.z < minFollowDist) {
            curOffset.z = minFollowDist;
        }
        curOffset.z = Mathf.Lerp(curOffset.z, -baseOffset.z, Time.deltaTime * sensitivity);

        //Apply to position
        transform.position = playerPos - transform.TransformVector(curOffset);

        //Preserve camera height above body
        Vector3 camNormal = player.nearestBody.NormalFor(transform.position);

        //Set rotation to be normal to the body
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(playerPos - transform.position, camNormal), camNormal);

        var yOffset = transform.InverseTransformVector(player.nearestBody.transform.position - transform.position);
        //yOffset.y = -yFromBody;

        //Apply to position
        transform.position = player.nearestBody.transform.position - transform.TransformVector(yOffset);

        //Set rotation to be tangent to the player again
        transform.rotation = realRot;

        //Follow above player
        curOffset = transform.InverseTransformVector(playerPos - transform.position);

        if (curOffset.y < -maxHeightFromPlayerFeet) {
            curOffset.y = -maxHeightFromPlayerFeet;
        } else if(curOffset.y > -minHeightFromPlayerFeet) {
            curOffset.y = -minHeightFromPlayerFeet;
        }
        curOffset.y = Mathf.Lerp(curOffset.y, -baseOffset.y, Time.deltaTime * sensitivity);

        //Apply to position
        transform.position = playerPos - transform.TransformVector(curOffset);

        //Point the camera at the player
        mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, Quaternion.LookRotation(playerPos - mainCam.transform.position, playerNormal), Time.deltaTime * sensitivity * 3);

        //Debug.DrawRay(playerPos, player.transform.up, Color.red, 4);
        //Debug.DrawLine(playerPos, mainCam.transform.position, Color.blue, 4);
    }
}
