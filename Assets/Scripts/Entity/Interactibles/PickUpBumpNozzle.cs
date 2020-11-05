using UnityEngine;
using System.Collections;

public class PickUpBumpNozzle : MonoBehaviour {

    // Use this for initialization
    void Start() {
        var npc = GetComponent<NPCController>();
        npc.OnInteract += OnInteract;
    }

    protected void OnInteract(PlayerController player, NPCController npc) {
        player.GetComponent<PlayerAbility>().hasBumpNozzle = true;
    }

}
