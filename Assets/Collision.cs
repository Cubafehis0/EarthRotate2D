using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Collision : MonoBehaviour
{
    Emergency emergency;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        RegionControl region = collision.gameObject.GetComponent<RegionControl>();
        if(region.region==Region.Sea)
        {
            region.FloodAround();
        }
        else
        {
            region.changeRegionTo(Region.ironGround);
        }
        emergency.hasEmergency = false;
        emergency.line.SetPosition(1, Vector3.zero);
        emergency.fixedText.text = null;
        emergency.secondText.text = null;
        emergency.hasYunShi = false;
        Destroy(gameObject);
    }
    private void Awake()
    {
        emergency = Emergency.emergency;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
