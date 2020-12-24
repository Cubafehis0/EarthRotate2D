using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempRoatate : MonoBehaviour
{
    Earth earth;
    // Start is called before the first frame update
    void Start()
    {
        earth = Earth.earth;
    }
    private void FixedUpdate()
    {
        transform.rotation = earth.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
