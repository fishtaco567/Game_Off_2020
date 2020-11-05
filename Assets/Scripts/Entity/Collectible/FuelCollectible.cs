using UnityEngine;
using System.Collections;

public class FuelCollectible : CollectibleBase {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    protected override void OnCollected(PlayerController player, PlayerAbility playerAbility) {
        playerAbility.numFuel++;

        player.OnCollect(this);

        Destroy(this.gameObject);
    }
}
