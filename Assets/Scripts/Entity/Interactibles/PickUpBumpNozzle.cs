using UnityEngine;
using System.Collections;

public class PickUpBumpNozzle : MonoBehaviour {

    public int pickUpOnBox;

    // Use this for initialization
    void Start() {
        var npc = GetComponent<NPCController>();
        npc.OnInteract += OnInteract;
        npc.OnDisplayBox += OnDisplayBox;
    }

    protected void OnInteract(PlayerController player, NPCController npc) {
        player.GetComponent<PlayerAbility>().hasBumpNozzle = true;
    }

    protected void OnDisplayBox(int i, NPCController npc) {
        if(i == pickUpOnBox) {
            npc.GetComponentInChildren<Animator>().SetTrigger("Give");
        }
    }

}
