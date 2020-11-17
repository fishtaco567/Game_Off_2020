using UnityEngine;
using System.Collections.Generic;

public class PlanetManager : Singleton<PlanetManager> {

    public List<GravityBody> planets;

    // Use this for initialization
    void Awake() {
        planets = new List<GravityBody>();
    }

    public void RegisterBody(GravityBody body) {
        planets.Add(body);
    }

    public GravityBody NearestBodyFor(FallBody faller) {
        var position = faller.transform.position;

        GravityBody nearestBody = planets[0];
        float minDistance = float.MaxValue;

        foreach(GravityBody body in planets) {
            var distance = Vector3.Distance(position, body.transform.position);

            if(distance < minDistance) {
                minDistance = distance;
                nearestBody = body;
            }
        }

        return nearestBody;
    }

    // Update is called once per frame
    void Update() {

    }
}
