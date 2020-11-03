using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace behaviors {
    class KillTrigger : MonoBehaviour {

        protected void OnTriggerEnter(Collider other) {
            var fallBody = other.GetComponent<FallBody>();
            if(fallBody != null) {
                fallBody.Die();
            }
        }

    }
}
