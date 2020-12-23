using System.Collections;
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
    bool hasFirstCity;
    public Text eraText;
    public Text populationText;
    public Sprite[] citySprites;
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
        rotateControl = RotateControl.rotateControl;
        regionControls = GetComponentsInChildren<RegionControl>();
        hasFirstCity = false;
    }
    private void FixedUpdate()
    {
        FirstCity();
        Population();  
    }
    void Population()
    {
        if(hasFirstCity)
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
                    eraText.text = "原子能时代";
                    break;
                }
        }
        CityUpDate();
    }
    void CityUpDate()
    {
        foreach(var city in cityRedenerer)
        {
            city.sprite = citySprites[(int)era];
        }
    }
    bool NewCity()
    {
        if(era==Era.AgricultureEra)
        {
            foreach (var region in regionControls)
            {
                if (region.region == Region.Forest || region.region == Region.FlatGround)
                {
                    GameObject newCity = GameObject.Instantiate(city, region.gameObject.transform);
                    newCity.transform.localPosition = new Vector3(0, -2.3f, 0);
                    SpriteRenderer renderer = newCity.GetComponent<SpriteRenderer>();
                    cityRedenerer.Add(renderer);
                    renderer.sprite = citySprites[(int)era];
                    region.region = Region.City;
                    return true;
                }
            }
        }
        else
        {
            foreach (var region in regionControls)
            {
                if (region.region != Region.City)
                {
                    GameObject newCity = GameObject.Instantiate(city, region.gameObject.transform);
                    city.transform.localPosition = new Vector3(0, -2.3f, 0);
                    region.region = Region.City;
                    return true;
                }
            }
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
