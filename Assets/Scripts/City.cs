using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    RegionControl regionControl;
    Earth earth = Earth.earth;
    // Start is called before the first frame update
    void Start()
    {
        regionControl = GetComponent<RegionControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
