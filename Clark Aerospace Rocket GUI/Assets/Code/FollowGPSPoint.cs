using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
public class FollowGPSPoint : MonoBehaviour
{

    public float lat = 0f;
    public float lon = 0f;
    public AbstractMap map;

    void Update()
    {
        transform.position = GeneralManager.manager.GetCurrentMap().GeoToWorldPosition(new Mapbox.Utils.Vector2d(lat, lon), false);
    }
}
