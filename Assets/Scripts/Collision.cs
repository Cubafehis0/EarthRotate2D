using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Collision : MonoBehaviour
{
    Emergency emergency;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Region") || collision.gameObject.layer == LayerMask.NameToLayer("Barrier"))
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
                    if (region.nowAAG != null)
                    {
                        region.nowAAG.SetActive(false);
                        region.nowAAG = null;
                    }
                    region.changeRegionTo(Region.ironGround);
                }
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Barrier"))
            {


            }
            emergency.warningLine.SetPosition(0, Vector3.zero);
            emergency.warningLine.SetPosition(1, Vector3.zero);
            emergency.hasEmergency = false;
            emergency.hasYunShi = false;
            Destroy(gameObject);
        }
    }
    private void Awake()
    {
        emergency = Emergency.emergency;
    }
}
