using UnityEngine;
using System.Collections;

public class MoneyCollectible : CollectibleBase {

    bool collected;

    // Use this for initialization
    void Start() {
        collected = false;
    }

    // Update is called once per frame
    void Update() {

    }
    protected override void OnCollected(PlayerController player, PlayerAbility playerAbility) {
        if(!collected) {
            collected = true;
            playerAbility.money++;
        }

        player.OnCollect(this);

        Destroy(this.gameObject);
    }
}
