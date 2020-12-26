using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGGManager : MonoBehaviour
{
    RegionControl RegionController;
    Earth earthInstance;
    Emergency emergencyInstance;
    int cityLevel = -1;
    public List<GameObject> AGGs;

    // Start is called before the first frame update
    void Start()
    {
        RegionController = GetComponentInParent<RegionControl>();
        earthInstance = Earth.earth;
        emergencyInstance = Emergency.emergencyInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if (earthInstance.GetCityLevel(RegionController) != cityLevel)
        {
            cityLevel = earthInstance.GetCityLevel(RegionController);
        }
        
    }

    public void setAllAGGDeavtive()
    {
        foreach (var AGG in AGGs)
        {
            AGG.SetActive(false);
        }
    }

    public void setAGGActive(int cityLevel)
    {
        setAllAGGDeavtive();
        AGGs[cityLevel-1].SetActive(true);
    }

    public void setAGGDeactive(int cityLevel)
    {
        AGGs[cityLevel-1].GetComponent<Animator>().SetTrigger("end");
    }
}
