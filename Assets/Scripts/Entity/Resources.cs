using UnityEngine;
using System.Collections;
using System;

public class Resources : MonoBehaviour {

    [SerializeField]
    private int health;

    [SerializeField]
    private int maxHealth;

    [SerializeField]
    private int iFrameTime;

    private float timeSinceLastHit;

    public int Health {
        get {
            return health;
        }

        set {
            if(timeSinceLastHit < iFrameTime) {
                if(value <= 0) {
                    OnDeath?.Invoke();
                    health = Math.Max(0, value);

                    return;
                }

                if(value < health) {
                    OnHit?.Invoke(health - value);
                    timeSinceLastHit = 0;
                } else if(value > health) {
                    OnHeal?.Invoke(value - health);
                }

                if(value > maxHealth) {
                    value = maxHealth;
                }

                health = value;
            }
        }
    }

    public Action<float> OnHit;

    public Action<float> OnHeal;

    public Action OnDeath;

    protected void Start() {
        timeSinceLastHit = iFrameTime;
    }

    protected void Update() {
        timeSinceLastHit += Time.deltaTime;
    }

    public bool ChangeHealth(int delta, bool respectIFrames = true, bool triggerDelegates = true) {
        if(delta == 0) {
            return false;
        } else if(delta < 0) {
            if(respectIFrames) {
                if(health - delta <= 0) {
                    if(triggerDelegates) {
                        OnDeath?.Invoke();
                    }
                    health = Math.Max(0, health - delta);
                    return true;
                }

                if(timeSinceLastHit < iFrameTime) {
                    health += delta;
                    if(triggerDelegates) {
                        OnHit?.Invoke(delta);
                    }
                    timeSinceLastHit = 0;
                    return true;
                } else {
                    return false;
                }
            } else {
                health += delta;
                if(triggerDelegates) {
                    OnHit?.Invoke(delta);
                }
                return true;
            }

        } else if(delta > 0) {
            health += delta;

            if(triggerDelegates) {
                OnHeal?.Invoke(delta);
            }
            return true;
        }

        //Unreachable code??
        return false;
    }

}
