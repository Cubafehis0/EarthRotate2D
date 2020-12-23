using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignRegion : MonoBehaviour
{
    List<Region> regions = new List<Region>();
    public int forestCnt;
    public int desertCnt;
    public int seaCnt;
    public int flatGroundCnt;
    // Start is called before the first frame update
    void Start()
    {
        AddRegion(Region.FlatGround, flatGroundCnt);
        AddRegion(Region.Desert, desertCnt);
        AddRegion(Region.Sea, seaCnt);
        AddRegion(Region.Forest, forestCnt);
        Assign();
    }
    void AddRegion(Region region ,int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            regions.Add(region);
        }
    }
    void Assign()
    {
        RegionControl[] regionControls = GetComponentsInChildren<RegionControl>();
        foreach (var reCont in regionControls)
        {
            int cnt =(int)(Random.Range(0,regions.Count));
            Region region = regions[cnt];
            reCont.region = region;
            regions.Remove(region);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
