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

    private PlayerAbility playerAbility;

    protected void OnInteract(PlayerController player, NPCController npc) {
        playerAbility = player.GetComponent<PlayerAbility>();
    }

    protected bool OnDisplayBox(int i, NPCController npc) {
        if(i == pickUpOnBox) {
            if(playerAbility.hasBumpNozzle != true) {
                npc.GetComponentInChildren<Animator>().SetTrigger("Give");
                playerAbility.hasBumpNozzle = true;
                UIManager.Instance.ObtainBump();
            }
        }

        return true;
    }

}
