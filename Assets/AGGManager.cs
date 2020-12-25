using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGGManager : MonoBehaviour
{
    RegionControl RegionController;
    public GameObject IndustryAGG;

    // Start is called before the first frame update
    void Start()
    {
        RegionController = GetComponentInParent<RegionControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
