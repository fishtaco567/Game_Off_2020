using UnityEngine;
using System.Collections;

public class PlayerAbility : MonoBehaviour {

    public int numFuel;

    public bool hasRestrictorNozzle;

    public bool hasBumpNozzle;

    public int money;

    public int maxHealth;

    public int currentHealth;

    public void TakeDamage(int damage) {
        var newHealth = currentHealth - damage;
        if(newHealth < 0) {
            Die();
            currentHealth = 0;
        } else {
            currentHealth = newHealth;
        }
    }

    public void Update() {
        UIManager.Instance.UpdateFuelText(numFuel);
        UIManager.Instance.UpdateMoneyText(money);
    }

    public void Die() {
    
    }

    public void ResetHealth() {
        currentHealth = maxHealth;

    }

}