using UnityEngine;
using System.Collections;

public class PickUpRestrictorNozzle : MonoBehaviour {

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
            if(playerAbility.hasRestrictorNozzle != true) {
                npc.GetComponentInChildren<Animator>().SetTrigger("Give");
                playerAbility.hasRestrictorNozzle = true;
                UIManager.Instance.ObtainRestrictor();
            }
        }

        return true;
    }

}
