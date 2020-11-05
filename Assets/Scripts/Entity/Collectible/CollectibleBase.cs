using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleBase : MonoBehaviour {

    protected void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<PlayerController>();
        var playerAbility = other.GetComponent<PlayerAbility>();
        
        if(player != null && playerAbility != null) {

        }
    }

    protected abstract void OnCollected(PlayerController player, PlayerAbility playerAbility);

}
