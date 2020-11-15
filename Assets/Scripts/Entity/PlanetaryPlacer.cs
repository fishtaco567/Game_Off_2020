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

    private void OnValidate() {
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
