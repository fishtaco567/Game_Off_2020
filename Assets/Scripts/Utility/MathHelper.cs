using UnityEngine;

public class MathHelper {

    public static Vector2 AngleToVector2(float angle, float length) {
        float angleInDeg = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInDeg), Mathf.Sin(angleInDeg)) * length;
    }

    public static Vector2 RadAngleToVector2(float angle, float length) {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length;
    }

    public static float Lerp(float a, float b, float x) {
        return a + (b - a) * x;
    }
    public static Vector2 Lerp(Vector2 a, Vector2 b, Vector2 x) {
        return a + (b - a) * x;
    }

}
