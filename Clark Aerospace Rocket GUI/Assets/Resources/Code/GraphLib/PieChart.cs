using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{

    public Material part1;
    public Material part2;

    public float ratio = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        part1.SetFloat("_StartAngle", 0f);
        part1.SetFloat("_EndAngle", ratio);

        part2.SetFloat("_StartAngle", ratio);
        part2.SetFloat("_EndAngle", Mathf.PI * 2);
    }
}
