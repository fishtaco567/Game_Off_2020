using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FireShooter : MonoBehaviour {

    [SerializeField]
    float delay;
    [SerializeField]
    float onTime;
    [SerializeField]
    int damage;

    [SerializeField]
    ParticleSystem system1;
    [SerializeField]
    ParticleSystem system2;

    ParticleSystem.EmissionModule system1Em;
    ParticleSystem.EmissionModule system2Em;

    [SerializeField]
    bool isOn;

    float currentTime;

    [SerializeField]
    AudioSource audio;

    public void Start() {
        system1Em = system1.emission;
        system2Em = system2.emission;

        currentTime = 0;
    }

    public void Update() {
        currentTime += Time.deltaTime;
        if(isOn && currentTime > onTime) {
            isOn = false;
            currentTime = 0;
        } else if(!isOn && currentTime > delay) {
            isOn = true;
            currentTime = 0;
        }

        if(isOn) {
            system1Em.enabled = true;
            system2Em.enabled = true;
            if(!audio.isPlaying) {
                audio.Play();
            }
        } else {
            system1Em.enabled = false;
            system2Em.enabled = false;
            audio.Stop();
        }
    }

    public void OnTriggerEnter(Collider other) {
        if(!isOn) {
            return;
        }

        var player = other.GetComponent<PlayerController>();
        var ability = other.GetComponent<PlayerAbility>();

        if(player != null && ability != null) {
            player.TimeOut();
            ability.TakeDamage(damage);
        }
    }

}
