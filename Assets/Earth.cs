﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Earth : MonoBehaviour
{
    public static Earth earth;
    public GameObject city;
    public float minS;
    public float maxS;
    public float time;
    public float occurTime;
    public int polInc;
    public int maxPol;
    public int maxCity;
    public int pol;
    public bool hasFirstCity;
    public Text eraText;
    public Text populationText;
    public Sprite[] citySprites;
    public float polNormalDecProportion;
    public float polOverTempDecProportion;
    List<SpriteRenderer> cityRedenerer;

    RotateControl rotateControl;
    RegionControl[] regionControls;
    Era era;
    private void Awake()
    {
        if(earth!=null)
        {
            Destroy(earth);
        }
        earth = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        cityRedenerer = new List<SpriteRenderer>();
        rotateControl = RotateControl.rotateControlInstance;
        regionControls = GetComponentsInChildren<RegionControl>();
        hasFirstCity = false;
    }
    private void FixedUpdate()
    {
        FirstCity();
        if(hasFirstCity)
        {
            PopulationInc();
            Population();
        } 
    }
    void PopulationInc()
    {
        int notFullPolCityCnt = 0;
        int shineCityCnt = 0;
        foreach (var region in regionControls)
        {
            if (region.region == Region.City)
            {
                if (!region.isOverNormalTemp())
                {
                    Debug.Log(region.pol);
                    if (region.isUnderSunshine())
                    {
                        shineCityCnt++;
                        if (region.pol < maxPol)
                        {
                            notFullPolCityCnt++;
                        }
                    }
                }
            }
        }

        if (notFullPolCityCnt != 0)
        {
            float speed = shineCityCnt / notFullPolCityCnt;
            foreach (var region in regionControls)
            {
                if (region.region == Region.City)
                {
                    if (!region.isOverNormalTemp())
                    {
                        if (region.isUnderSunshine())
                        {
                            if (region.pol < maxPol)
                            {
                                region.polF += polInc * speed * Time.deltaTime;
                            }
                        }
                    }
                }
            }
        }
        foreach (var region in regionControls)
        {
            if (region.region == Region.City)
            {
                //自然减少
                region.decreasePolF += polNormalDecProportion * region.polF * Time.deltaTime;
                //温度超过一定值，人口不增加，直接减少
                if (region.isOverNormalTemp())
                {
                    region.decreasePolF += polOverTempDecProportion * region.polF * Time.deltaTime;
                }
                region.pol = (int)region.polF;
                if (pol == maxPol)
                {
                    region.polF = region.pol;
                }
                region.decreasePol = (int)region.decreasePolF;
                if (region.decreasePol > 0)
                {
                    region.pol -= region.decreasePol;
                    region.polF -= region.decreasePol;
                    region.decreasePolF -= region.decreasePol;
                    region.decreasePol = 0;
                }
                if (region.pol < 0)
                {
                    region.pol = 0;
                    region.polF = 0;
                }
            }
            //CityUpDate();
        }
    }
    void Population()
    {
        pol = 0;
        int cityCnt = 0;
        bool isNeedNewCity = true;
        foreach (var region in regionControls)
        {
            if (region.region == Region.City)
            {
                cityCnt++;
                pol += region.pol;
                if (region.pol < maxPol)
                {
                    isNeedNewCity = false;
                }
            }
        }
        if (isNeedNewCity)
        {
            if (cityCnt == maxCity)
            {
                int m = (int)era;
                SwitchEra((Era)(m + 1));
            }
            else
            {
                NewCity();
            }
        }
        populationText.text = pol.ToString();        
    }
    void FirstCity()
    {
        if(!hasFirstCity)
        {
            if (Mathf.Abs(rotateControl.earthS) >= minS && Mathf.Abs(rotateControl.earthS) <= maxS)
            {
                time += Time.deltaTime;
            }
            else
            {
                time = 0;
            }
            if (time > occurTime)
            {
                if(NewCity())
                {
                    SwitchEra(Era.AgricultureEra);
                }
                time = 0;
                hasFirstCity = true;
            }
        }
    }
    void SwitchEra(Era era)
    {
        this.era = era;
        switch(era)
        {
            case Era.AgricultureEra:
                {
                    polInc = 1;
                    maxPol=10;
                    maxCity = 3;
                    eraText.text = "农业时代";
                    break;
                }
            case Era.IndutrialEra:
                {
                    polInc = 10;
                    maxCity = 6;
                    maxPol = 100;
                    eraText.text = "工业时代";
                    break;
                }
            case Era.InformationEra:
                {
                    polInc = 100;
                    maxPol = 1000;
                    maxCity = 9;
                    eraText.text = "信息时代";
                    break;
                }
            case Era.AtomicEra:
                {
                    polInc = 1000;
                    maxPol = 10000;
                    maxCity = 10000;
                    eraText.text = "原子能时代";
                    break;
                }
        }
    }
    void CityUpDate()
    {
        //foreach(var city in cityRedenerer)
        //{
        //    city.sprite = citySprites[(int)era];
        //}
        foreach(var region in regionControls)
        {
            if(region.region==Region.City)
            {
                int index = GetCityLevel(region);
                region.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite=citySprites[index];
            }
        }
    }
    int GetCityLevel(RegionControl region)
    {
        if(0<=region.pol && region.pol<10)
        {
            return 0;
        }
        else if(region.pol>=10 && region.pol<100)
        {
            return 1;
        }
        else if(region.pol>=100 && region.pol<1000)
        {
            return 2;
        }
        else if(region.pol>=1000 && region.pol<=10000)
        {
            return 3;
        }
        return -1;
    }
    void CityUpdate(SpriteRenderer renderer)
    {
        renderer.sprite = citySprites[(int)era];
    }
    bool NewCity()
    {
        GameObject newCity=null;
        RegionControl region1=null;
        if(era==Era.AgricultureEra)
        {
            foreach (var region in regionControls)
            {
                if (region.region == Region.Forest || region.region == Region.FlatGround)
                {
                    region1 = region;
                    break;
                }
            }
        }
        else
        {
            foreach (var region in regionControls)
            {
                if (region.region != Region.City)
                {
                    region1 = region;
                    break;
                }
            }
        }
        if(region1==null)
        {
            return false;
        }
        newCity = GameObject.Instantiate(city, region1.gameObject.transform);
        newCity.transform.localPosition = new Vector3(0, -2.3f, 0);
        SpriteRenderer renderer = newCity.GetComponent<SpriteRenderer>();

        //cityRedenerer.Add(renderer);
        //renderer.sprite = citySprites[(int)era];
        region1.region = Region.City;
        region1.pol = 1;
        region1.polF = 1;
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
