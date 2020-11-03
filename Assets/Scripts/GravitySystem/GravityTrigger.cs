using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace behaviors {
    class GravityTrigger : MonoBehaviour {

        [SerializeField]
        GravityBody gravityBody;

        protected void OnTriggerStay(Collider other) {
            var fallBody = other.GetComponent<FallBody>();
            if (fallBody != null && fallBody.nearestBody == null) {
                fallBody.nearestBody = gravityBody;
            }
        }

        protected void OnTriggerExit(Collider other) {
            /*var fallBody = other.GetComponent<FallBody>();
            if (fallBody != null) {
                fallBody.nearestBody = null;
            }*/
        }

    }
}
