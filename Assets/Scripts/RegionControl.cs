using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionControl : MonoBehaviour
{
    [Tooltip("异常温度值")]
    public float abnormalTemperature;
    [Tooltip("异常温度值改变状态需要的累计时间")]
    public float changeTime;
    // 高温累计时间
    float temperatureToohighTime;
    // 低温累计时间
    float temperatureToolowTime;
    public float temperatureUpRatio;
    public float initTemperature;
    public float temperature;
    //人口可以增长的两个临界值
    public float polMinTemp;
    public float polMaxTemp;
    public Region region;
    public float decreasePolF;
    public int decreasePol;
    public float polF;
    public int pol;
    SpriteRenderer sprite;
    Earth earth;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        LoadImage();
        temperature = initTemperature;
        earth = Earth.earth;
        temperatureToohighTime = 0f;
    }

    void LoadImage()
    {
        switch (region)
        {
            case Region.Desert:
                {
                    sprite.color = Color.yellow;
                    break;
                }
            case Region.FlatGround:
                {
                    sprite.color = Color.grey;
                    break;
                }
            case Region.Forest:
                {
                    sprite.color = Color.green;
                    break;
                }
            case Region.Sea:
                {
                    sprite.color = Color.blue;
                    break;
                }
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(region==Region.City && isReachDestoryTemp())
        {
            region = Region.Desert;
            Transform transform1=transform.GetChild(0);
            Destroy(transform1.gameObject);
            pol = 0;
            polF = 0;
            decreasePol = 0;
            decreasePolF = 0;
        }
        if (isUnderSunshine())
        {
            temperature += temperatureUpRatio * Time.fixedDeltaTime;
        }
        else
        {
            temperature -= temperatureUpRatio * Time.fixedDeltaTime;
        }
        // 计算异常时间累计值
        if (temperature > abnormalTemperature)
        {
            temperatureToohighTime += Time.fixedDeltaTime;
        }
        else if (temperature < -abnormalTemperature)
        {
            temperatureToolowTime += Time.fixedDeltaTime;
        }
        else
        {
            temperatureToohighTime -= Time.fixedDeltaTime;
            temperatureToohighTime = Mathf.Clamp(temperatureToohighTime, 0f, float.MaxValue);
            temperatureToolowTime -= Time.fixedDeltaTime;
            temperatureToolowTime = Mathf.Clamp(temperatureToolowTime, 0f, float.MaxValue);
        }
        // 温度过高一律变成沙漠
        if (temperatureToohighTime >= changeTime)
        {
            changeRegionToDesert();
            
        }
        // 温度过低，海洋不变沙漠
        if (temperatureToolowTime >= changeTime && region != Region.Sea)
        {
            changeRegionToDesert();
        }

    }

    public bool isUnderSunshine()
    {
        if (transform.rotation.eulerAngles.z >= 45f && transform.rotation.eulerAngles.z < 225f)
        {
            return true;
        }
        return false;
    }
    public bool isOverNormalTemp()
    {
        if(temperature>10 || temperature<-10)
        {
            return true;
        }
        return false;
    }
    public bool isReachDestoryTemp()
    {
        if(temperature>15 || temperature<-15)
        {
            return true;
        }
        return false;
    }
    private void changeRegionToDesert()
    {
        region = Region.Desert;
        LoadImage();
    }
    
}
