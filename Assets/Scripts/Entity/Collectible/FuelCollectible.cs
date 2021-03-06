﻿using UnityEngine;
using System.Collections;

public class FuelCollectible : CollectibleBase {

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
            playerAbility.numFuel++;
        }

        player.OnCollect(this);

        Destroy(this.gameObject);
    }
}
