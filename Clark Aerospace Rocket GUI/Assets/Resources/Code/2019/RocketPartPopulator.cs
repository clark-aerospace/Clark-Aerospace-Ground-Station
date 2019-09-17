using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPartPopulator : MonoBehaviour
{
    public static RocketPartPopulator populator;
    public List<RocketComponent> rocketComponents;

    public float offset;
    public Transform location;

    public GameObject itemPrefab;

    public GameObject measurementPrefab;

    void Awake() {
        if (populator != null) {Destroy(gameObject);}
        else {populator = this;}
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform trans in location) {
            Destroy(trans.gameObject);
        }

        foreach (RocketComponent comp in rocketComponents) {
            GameObject newi = Instantiate(itemPrefab);
            newi.transform.SetParent(location, false);
            // newi.GetComponent<RectTransform>().anchoredPosition = new Vector2(10f, offset);
            // offset -= 90;

            newi.GetComponent<RocketPartItem>().SetComponent(comp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
