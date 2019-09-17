using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeMath
{
    public readonly static float EARTH_RADIUS_KM = 6371f;

    public readonly static float KM_TO_MI = 0.62137f;

    /// <summary>
    /// Calculates the distance between two points on the Earth's surface
    /// </summary>
    public static float Haversine(Vector2 point1, Vector2 point2, DistanceUnits u = DistanceUnits.Kilometers) {
        float lat1 = point1.x * Mathf.Deg2Rad;
        float lat2 = point2.x * Mathf.Deg2Rad;
        float deltaLat = (point2.x - point1.x) * Mathf.Deg2Rad;
        float deltaLong = (point2.y - point1.y) * Mathf.Deg2Rad;

        float a = Mathf.Pow(Mathf.Sin(deltaLat / 2), 2)     +     Mathf.Cos(lat1) * Mathf.Cos(lat2)      *      Mathf.Pow(Mathf.Sin(deltaLong / 2), 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        float d = EARTH_RADIUS_KM * c;

        if (u == DistanceUnits.Miles) d = KilometersToMiles(d);

        return d;
    }

    /// <summary>
    /// Calculates the bearing between two points on the Earth's surface
    /// </summary>
    public static float BearingToPoint(Vector2 current, Vector2 target) {

        float lat1 = current.x * Mathf.Deg2Rad;
        float lat2 = target.x * Mathf.Deg2Rad;

        float long1 = current.y * Mathf.Deg2Rad;
        float long2 = target.y * Mathf.Deg2Rad;

        float deltaLong = (long2 - long1);

        float yComp = Mathf.Sin(deltaLong) * Mathf.Cos(lat2);
        float xComp = Mathf.Cos(lat1) * Mathf.Sin(lat2) - Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(deltaLong);
        float angle = Mathf.Atan2(yComp, xComp) * Mathf.Rad2Deg;

        //Debug.Log("Bearing to point is " + angle.ToString() + " degrees");

        return angle;
    }

    public static float SimpleDistanceToHorizon(Vector2 current, float elevation) {
        return Mathf.Sqrt((2 * elevation * EARTH_RADIUS_KM) + Mathf.Pow(elevation, 2));
    }

    public static float KilometersToMiles(float km) {
        return km * KM_TO_MI;
    }



}

public enum DistanceUnits {
    Kilometers = 0,
    Miles = 1
}
