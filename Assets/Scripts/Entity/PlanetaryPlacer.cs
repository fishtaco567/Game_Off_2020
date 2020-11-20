using UnityEngine;
using System.Collections;

public class PlanetaryPlacer : MonoBehaviour {

    [SerializeField]
    private GravityBody planet;

    [SerializeField]
    private float radius;
    [SerializeField]
    private float phi;
    [SerializeField]
    private float theta;

    [SerializeField]
    private float heightAbove;

    [SerializeField]
    private bool setToHeight;

    private void OnValidate() {
        if(setToHeight) {
            RaycastHit hit;
            if(Physics.Raycast(transform.position + transform.up * 30, -transform.up, out hit, 60, LayerMask.GetMask("Terrain"))) {
                radius = Vector3.Distance(hit.point, planet.transform.position) + heightAbove;
            }

        }

        float x = radius * Mathf.Sin(theta * Mathf.Deg2Rad) * Mathf.Cos(phi * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(theta * Mathf.Deg2Rad) * Mathf.Sin(phi * Mathf.Deg2Rad);
        float z = radius * Mathf.Cos(theta * Mathf.Deg2Rad);

        var vec = new Vector3(x, y, z);
        vec += planet.transform.position;

        var rot = planet.NormalFor(vec);

        transform.position = vec;
        transform.up = rot;
    }
}
