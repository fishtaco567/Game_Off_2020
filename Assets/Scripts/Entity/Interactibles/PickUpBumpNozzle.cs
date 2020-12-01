using UnityEngine;
using System.Collections;

public class PickUpBumpNozzle : MonoBehaviour {

    public int pickUpOnBox;

    private bool hasPickedUp;

    // Use this for initialization
    void Start() {
        var npc = GetComponent<NPCController>();
        npc.OnInteract += OnInteract;
        npc.OnDisplayBox += OnDisplayBox;
        npc.PlaySecond += Second;
        hasPickedUp = false;
    }

    private PlayerAbility playerAbility;

    protected bool Second(NPCController npc) {
        return hasPickedUp;
    }

    protected void OnInteract(PlayerController player, NPCController npc) {
        playerAbility = player.GetComponent<PlayerAbility>();
    }

    protected bool OnDisplayBox(int i, NPCController npc) {
        if(i == pickUpOnBox) {
            if(playerAbility.hasBumpNozzle != true) {
                hasPickedUp = true;
                npc.GetComponentInChildren<Animator>().SetTrigger("Give");
                playerAbility.hasBumpNozzle = true;
                UIManager.Instance.ObtainBump();
            }
        }

        return true;
    }

}
