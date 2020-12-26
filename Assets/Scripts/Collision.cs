using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Collision : MonoBehaviour
{
    Emergency emergency;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Region"))
        {
            RegionControl region = collision.gameObject.GetComponent<RegionControl>();
            if (region.region == Region.Sea)
            {
                region.FloodAround();
            }
            else
            {
                region.changeRegionTo(Region.ironGround);
                region.nowMineTime = region.mineTime;
                region.SetAlternatorActive(false);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {

        }
        emergency.hasEmergency = false;
        emergency.hasYunShi = false;
        Destroy(gameObject);
    }
    private void Awake()
    {
        emergency = Emergency.emergency;
    }
}
