using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Earth : MonoBehaviour
{
    #region("成员变量")
    public int badEndSceneInd = 2;
    public int goodEndSceneInd = 3;
    public static Earth earth;
    public float minS;
    public float maxS;
    float firstCityTime;
    [Tooltip("速度稳定时出现第一个城市需要的时间")]
    public float occurTime;
    //是否拥有第一个城市
    bool hasFirstCity;
    public Text eraText;
    public Text populationText;
    public float polNormalDecProportion;
    public float polOverTempDecProportion;
    public float polOverSpeedDecProportion;
    public float earthSpeedToDeacresePeople;

    bool hasLaunchSatellite;
    public GameObject satellite;
    public Vector3 satalliteStartPos;

    bool endGame;
    public GameObject endGameBt;
    
    public int pol;

    //游戏内任务线

    //人口是否超过50，进入工业化的条件
    //是否打败过外星人，进入信息化的条件
    //信息化城市是否超过3个，进入原子化
    public bool[] hasFinishEraTask= { false, false, false,false };
    //是否完成时代进化
    bool[] hasFinishEra = { false, false, false };

    [SerializeField]
    float eraEvolutionNeedTime=0;
    float eraHasEvolutionTime=0;
    [SerializeField]
    Text tip;

    [SerializeField]
    int[] populationInc;
    [SerializeField]
    int[] maxPopulationInEra;
    [SerializeField]
    int[] maxCityNum;
    [SerializeField]
    string[] eraTextContent;
    int maxPopulation;
    int maxCity;
    RegionSprite regionSprite;
    RotateControl rotateControl;
    public RegionControl[] regionControls;
    //完成时代化后，科技时代发生进化，科技时代不会发生变化
    public Era era;

    EventTip eventTip;
    #endregion
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
        rotateControl = RotateControl.rotateControlInstance;
        eventTip = EventTip.eventTip;
        regionControls = GetComponentsInChildren<RegionControl>();
        hasFirstCity = false;
        endGame = false;
        regionSprite = RegionSprite.regionSprite;
        hasLaunchSatellite = false;
    }
    private void FixedUpdate()
    {
        FirstCity();
        if(hasFirstCity)
        {
            if (endGame)
            {
                endGameBt.SetActive(true);
            }
            else
            {
                PopulationInc();
                Population();
                EraChange();
            }
        }
    }
    void EraChange()
    {
        if(!hasFinishEraTask[0])
        {
            if(pol>50)
            {
                hasFinishEraTask[0] = true;
            }
        }
        // 收炮


        if (!hasFinishEraTask[2])
        {
            int cnt = 0;
            foreach(var region in regionControls)
            {
                if(region.region==Region.City || region.region==Region.SeaCity)
                {
                    int level = GetCityLevel(region);
                    if(level==2)
                    {
                        cnt++;
                    }                    
                }
            }
            if(cnt>=3)
            {
                hasFinishEraTask[2] = true;
            }
        }
        //是否需要升级城市,当人口超过当前时代上限时解除第一个限制
        bool isSwitchEra=false;
        foreach(var region in regionControls)
        {
            if(region.pol==maxPopulation)
            {
                isSwitchEra = true;
                break;
            }
        }
        if(isSwitchEra)
        {
            int eraLevel = (int)era;
            if(hasFinishEraTask[eraLevel])
            {
                EraEvolution();
            }
        }          
        //进入原子化时代后，关闭所有风力发电机并且发射卫星
        if(era==Era.AtomicEra && !hasLaunchSatellite)
        {
            foreach(var region in regionControls)
            {
                if(GetCityLevel(region)==3 && !hasLaunchSatellite)
                {
                    LaunchSattllite(region);
                    hasLaunchSatellite = true;
                    break;
                }
            }
        }
    }
    public void EraEvolution()
    {
        //tip.text = "";
        eraHasEvolutionTime += Time.deltaTime;
        if(eraHasEvolutionTime>eraEvolutionNeedTime)
        {
            eraHasEvolutionTime = 0;
            tip.text = null;
            //hasFinishEra[(int)era+1] = true;
            SwitchEra((Era)((int)era+1));
        }
    }
    void PopulationInc()
    {
        //如果处于原子能时代，则去除所有负面影响
        foreach (var region in regionControls)
        {
            if (region.region == Region.City)
            {
                //自然减少
                region.decreasePolF += polNormalDecProportion * region.polF * Time.deltaTime;
                //温度超过一定值，人口不增加，直接减少
                if (region.isOverNormalTemp() && era<Era.AtomicEra)
                {
                    region.decreasePolF += polOverTempDecProportion * region.polF * Time.deltaTime;
                }
                region.decreasePol = (int)region.decreasePolF;
                if (region.decreasePol > 0)
                {
                    region.pol -= region.decreasePol;
                    region.polF -= region.decreasePol;
                    region.decreasePolF -= region.decreasePol;
                    region.decreasePol = 0;
                }
            }
        }
        //自转过快
        if (Mathf.Abs(rotateControl.earthS) > earthSpeedToDeacresePeople && era<Era.AtomicEra)
        {
            foreach (RegionControl region in regionControls)
            {
                if (region.region == Region.City || region.region == Region.SeaCity)
                {
                    region.decreasePolF += polOverSpeedDecProportion * region.polF * Time.deltaTime;
                }
                region.decreasePol = (int)region.decreasePolF;
                if (region.decreasePol > 0)
                {
                    region.pol -= region.decreasePol;
                    region.polF -= region.decreasePol;
                    region.decreasePolF -= region.decreasePol;
                    region.decreasePol = 0;
                }
            }
        }
        //正常人口增长
        else
        {
            //该参数记录了其他城市超出上限的增加的人口值
            float overPopulation = 0;
            //人数未满并且可以增加人口的城市
            int notFullCityCnt=0;
            foreach (var region in regionControls)
            {
                if (region.region == Region.City)
                {
                    if (!region.isOverNormalTemp() || era==Era.AtomicEra)
                    {
                        if (region.isUnderSunshine() || era==Era.AtomicEra)
                        {
                            //第一次遍历将将满的城市加满
                            if (region.polF + populationInc[GetCityLevel(region)] * Time.deltaTime > maxPopulation)
                            {
                                overPopulation += region.polF + populationInc[GetCityLevel(region)] * Time.deltaTime - maxPopulation;
                                region.polF = maxPopulation;
                                region.pol = maxPopulation;
                            }
                            else
                            {
                                notFullCityCnt++;
                            }
                        }
                        else
                        {
                            if(region.polF<maxPopulation)
                            {
                                notFullCityCnt++;
                            }
                        }
                    }
                }
            }
            if(notFullCityCnt!=0)
            {
                float incPopEveryCity = overPopulation / notFullCityCnt;
                foreach (var region in regionControls)
                {
                    if (region.region == Region.City)
                    {
                        if (!region.isOverNormalTemp() || era==Era.AtomicEra)
                        {
                            region.polF += incPopEveryCity;
                            if(region.isUnderSunshine() || Era.AtomicEra==era)
                            {
                                if (region.pol < maxPopulation)
                                {
                                    region.polF += populationInc[GetCityLevel(region)] * Time.deltaTime;
                                }
                            }
                        }
                    }
                }   
            }
        }
        foreach (var region in regionControls)
        {
            if (region.region == Region.City || region.region == Region.SeaCity)
            {
                region.pol = (int)region.polF;
                if (region.pol < 0)
                {
                    region.pol = 0;
                    region.polF = 0;
                }
                if(region.pol>=maxPopulation)
                {
                    region.pol = maxPopulation;
                    region.polF = region.pol;
                }
            }
        }
        CityUpDate();
    }
    void Population()
    {
        pol = 0;
        int cityCnt = 0;
        bool isNeedNewCity = true;
        foreach (var region in regionControls)
        {
            if (region.region == Region.City || region.region==Region.SeaCity)
            {
                cityCnt++;
                pol += region.pol;
                if (region.pol < maxPopulation)
                {
                    isNeedNewCity = false;
                }
            }
        }
        if (cityCnt == 0)
        {
            endGame = true;
            SceneManager.LoadScene(badEndSceneInd);
        }
        else if (isNeedNewCity)
        {
            if (cityCnt < maxCity)
            {
                NewCity();
            }
        }
        populationText.text = pol.ToString();    
        //人口超过221231
        if(pol>221231)
        {
            EventTip.eventTip.AddTips(Tip.AbserveSunExplosion);
        }
    }
    void FirstCity()
    {
        if(!hasFirstCity)
        {
            if (Mathf.Abs(rotateControl.earthS) >= minS && Mathf.Abs(rotateControl.earthS) <= maxS)
            {
                firstCityTime += Time.deltaTime;
            }
            else
            {
                firstCityTime = 0;
            }
            if (firstCityTime > occurTime)
            {
                eventTip.AddTips(Tip.PeopleBorn);
                if(NewCity())
                {
                    SwitchEra(Era.AgricultureEra);
                }
                firstCityTime = 0;
                hasFirstCity = true;
            }
        }
    }
    void SwitchEra(Era era)
    {
        switch (era)
        {
            case Era.AgricultureEra:
                {
                    eventTip.AddTips(Tip.AgriclutureEra);
                    break;
                }
            case Era.IndutrialEra:
                {
                    eventTip.AddTips(Tip.IndutrialEra);
                    eventTip.AddTips(Tip.DefenseWeapon);
                    break;
                }
            case Era.InformationEra:
                {
                    eventTip.AddTips(Tip.InformationEra);
                    eventTip.AddTips(Tip.DefenseTsunami);
                    break;
                }
            case Era.AtomicEra:
                {
                    eventTip.AddTips(Tip.AtomicEra);
                    eventTip.AddTips(Tip.DefenseMeteorite);
                    break;
                }

        }
        
        this.era = era;
        maxPopulation = maxPopulationInEra[(int)era];
        maxCity = maxCityNum[(int)era];
        eraText.text = eraTextContent[(int)era];
        Emergency.emergency.ChangYunShiGaiLv(era);
    }
    /// <summary>
    /// 根据城市的人口换贴图h或者摧毁城市
    /// </summary>
    void CityUpDate()
    {
        foreach(var region in regionControls)
        {
            if(region.region==Region.City ||region.region==Region.SeaCity)
            {
                int index = GetCityLevel(region);
                if(index==3)
                {
                    if(region.alternator==true)
                    {
                        region.SetAlternatorActive(false);
                    }
                }
                if(index!=-1)
                {
                    Sprite sprite = region.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
                    if(index==1)
                    {
                        if(sprite!= regionSprite.citySprites[1] && sprite!=regionSprite.citySprites[4])
                        {
                            if (region.nowAAG != null)
                            {
                                region.nowAAG.SetActive(false);
                            }
                            region.nowAAG = region.IndustryAAG;
                            float range = Random.value;
                            if (range < 0.5)
                            {
                                region.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = regionSprite.citySprites[1];
                            }
                            else
                            {
                                region.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = regionSprite.citySprites[4];
                            }
                        }
                    }
                    else
                    {
                        if(sprite!=regionSprite.citySprites[index])
                        {
                            if (region.nowAAG != null)
                            {
                                region.nowAAG.SetActive(false);
                                region.nowAAG = null;
                            }
                            if (index == 2)
                            {
                                region.nowAAG = region.InformationAAG;
                            }
                            else if (index == 3)
                            {
                                region.nowAAG = region.AtomicAAG;
                            }

                            region.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = regionSprite.citySprites[index];
                        }
                    }
                }
                else
                {
                    if (region.nowAAG != null)
                    {
                        region.nowAAG.SetActive(false);
                        region.nowAAG = null;
                    }
                    region.changeRegionTo(Region.FlatGround);
                }
            }
        }
    }
    public int GetCityLevel(RegionControl region)
    {
        if(0<region.pol && region.pol<=maxPopulationInEra[0])
        {
            return 0;
        }
        else if(region.pol> maxPopulationInEra[0] && region.pol<= maxPopulationInEra[1])
        {
            return 1;
        }
        else if(region.pol> maxPopulationInEra[1] && region.pol<= maxPopulationInEra[2])
        {
            return 2;
        }
        else if(region.pol> maxPopulationInEra[2] && region.pol<= maxPopulationInEra[3])
        {
            return 3;
        }
        return -1;
    }
    bool NewCity()
    {
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
                //优先找到不是陨石的地方建城市
                if (region.region != Region.City && region.region!=Region.SeaCity)
                {
                    region1 = region;
                    if(region1.region!=Region.ironGround)
                    {
                        break;
                    }
                }
            }
        }
        if(region1==null)
        {
            return false;
        }
        eventTip.AddTips(Tip.NewCity);
        SpriteRenderer renderer = region1.GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = regionSprite.citySprites[0];
        region1.region = Region.City;
        region1.pol = 1;
        region1.polF = 1;
        return true;
    }
    public void LaunchSattllite(RegionControl region)
    {
        GameObject Satellite = GameObject.Instantiate(satellite, region.transform, false);
    }
}
