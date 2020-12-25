﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Earth : MonoBehaviour
{
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
    RegionControl[] regionControls;
    //完成时代化后，科技时代发生进化，科技时代不会发生变化
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
        rotateControl = RotateControl.rotateControlInstance;
        regionControls = GetComponentsInChildren<RegionControl>();
        hasFirstCity = false;
        endGame = false;
        regionSprite = RegionSprite.regionSprite;
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
        //外星人

        //
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
        if (Mathf.Abs(rotateControl.earthS) > earthSpeedToDeacresePeople)
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
                    if (!region.isOverNormalTemp())
                    {
                        if (region.isUnderSunshine())
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
                            notFullCityCnt++;
                        }
                    }
                }
            }
            //if (notFullPolCityCnt != 0)
            //{
            //    float speed = shineCityCnt / notFullPolCityCnt;
            //    foreach (var region in regionControls)
            //    {
            //        if (region.region == Region.City)
            //        {
            //            if (!region.isOverNormalTemp())
            //            {
            //                if (region.pol < maxPol)
            //                {
            //                    region.polF += polInc * speed * Time.deltaTime;
            //                }
            //            }
            //        }
            //    }
            //}
            if(notFullCityCnt!=0)
            {
                float incPopEveryCity = overPopulation / notFullCityCnt;
                foreach (var region in regionControls)
                {
                    if (region.region == Region.City)
                    {
                        if (!region.isOverNormalTemp())
                        {
                            region.polF += incPopEveryCity;
                            if(region.isUnderSunshine())
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
        }
        else if (isNeedNewCity)
        {
            if (cityCnt < maxCity)
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
                firstCityTime += Time.deltaTime;
            }
            else
            {
                firstCityTime = 0;
            }
            if (firstCityTime > occurTime)
            {
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
        this.era = era;
        maxPopulation = maxPopulationInEra[(int)era];
        maxCity = maxCityNum[(int)era];
        eraText.text = eraTextContent[(int)era];
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
                if(index!=-1)
                {
                    Sprite sprite = region.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
                    if(index==1)
                    {
                        if(sprite!= regionSprite.citySprites[1] && sprite!=regionSprite.citySprites[4])
                        {
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
                            region.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = regionSprite.citySprites[index];
                        }
                    }
                }
                else
                {
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
        SpriteRenderer renderer = region1.GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = regionSprite.citySprites[0];
        region1.region = Region.City;
        region1.pol = 1;
        region1.polF = 1;
        return true;
    }
}