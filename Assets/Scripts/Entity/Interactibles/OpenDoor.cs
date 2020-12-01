using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

    public int openOnBox;
    public Door door;
    public int minMoney;

    public bool tookMoney;

    public bool opened;

    // Use this for initialization
    void Start() {
        var npc = GetComponent<NPCController>();
        npc.OnDisplayBox += OnDisplayBox;
        npc.OnInteract += OnInteract;
        npc.PlaySecond += Second;
        tookMoney = false;
        opened = false;
    }

    protected bool Second(NPCController npc) {
        return opened;
    }

    protected void OnInteract(PlayerController player, NPCController npc) {
        var ability = player.GetComponent<PlayerAbility>();
        if(ability != null && ability.money >= minMoney && !tookMoney) {
            ability.money -= minMoney;
            tookMoney = true;
        }
    }

    protected bool OnDisplayBox(int i, NPCController npc) {
        if(i == openOnBox && tookMoney) {
            return true;
        } else if(i == openOnBox && !tookMoney) {
            return false;
        } 

        if(i == openOnBox + 1 && tookMoney) {
            opened = true;
            door.SetOpen(true);
        }

        return true;
    }

}
