using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{
    public int rollTimes;
    public GameObject city;
    public float minS;
    public float maxS;
    public float time;
    public float occurTime;
    RotateControl rotateControl;
    RegionControl[] regionControls;

    // Start is called before the first frame update
    void Start()
    {
        rotateControl = RotateControl.rotateControl;
        regionControls = GetComponentsInChildren<RegionControl>();
    }
    private void FixedUpdate()
    {
        if(Mathf.Abs(rotateControl.earthS)>=minS && Mathf.Abs(rotateControl.earthS)<=maxS)
        {
            time += Time.deltaTime;
        }
        else 
        {
            time = 0;
        }
        if(time>occurTime)
        {
            Debug.Log(NewCity());
            time = 0;
        }
    }
    bool NewCity()
    {
        foreach(var region in regionControls)
        {
            if(region.region==Region.Forest || region.region==Region.FlatGround)
            {
                GameObject newCity = GameObject.Instantiate(city,region.gameObject.transform);
                city.transform.localPosition = new Vector3(0, 2.3f, 0);
                return true;
            }
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
