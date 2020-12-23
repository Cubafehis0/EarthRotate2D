using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionControl : MonoBehaviour
{
    [Tooltip("退潮需要的时间")]
    public float ebbTime;
    private float nowEbbTime;
    [Tooltip("异常加速度")]
    public float abnormalAc;
    [Tooltip("异常温度值")]
    public float abnormalTemperature;
    [Tooltip("异常温度值改变状态需要的累计时间")]
    public float changeTime;
    [Tooltip("最高温度")]
    public float maxTemperature;
    // 高温累计时间
    float temperatureToohighTime;
    // 低温累计时间
    float temperatureToolowTime;
    [Tooltip("温度改变速率")]
    public float temperatureUpRatio;
    [Tooltip("初始温度")]
    public float initTemperature;
    public float temperature;
    //人口可以增长的两个临界值
    public float polMinTemp;
    public float polMaxTemp;
    public Region region;
    public float polF;
    public int pol;
    SpriteRenderer sprite;
    Earth earth;
    RotateControl rotateControlInstance;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        LoadImage();
        temperature = initTemperature;
        earth = Earth.earth;
        temperatureToohighTime = 0f;
        rotateControlInstance = RotateControl.rotateControlInstance;
        nowEbbTime = 0f;
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
            case Region.SeaGround:
                {
                    sprite.color = Color.cyan;
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
        if (isUnderSunshine(transform.rotation.eulerAngles.z))
        {
            temperature += temperatureUpRatio * Time.fixedDeltaTime;
            if(region==Region.City && temperature>polMinTemp && temperature<polMaxTemp)
            {
                polF += earth.polInc * Time.deltaTime;
            }
            
        }
        else
        {
            temperature -= temperatureUpRatio * Time.fixedDeltaTime;
        }
        temperature = Mathf.Clamp(temperature, -maxTemperature, maxTemperature);

        // 
        pol = (int)polF;
        if(pol>earth.maxPol)
        {
            pol = earth.maxPol;
            polF = pol;
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
            changeRegionTo(Region.Desert);
            
        }
        // 温度过低，海洋和Sea Ground不变沙漠
        if (temperatureToolowTime >= changeTime && region != Region.Sea && region != Region.SeaGround)
        {
            changeRegionTo(Region.Desert);
        }


        // 海洋淹没
        Flood();

        // Sea Ground退潮
        Ebb();
    }

    private bool isUnderSunshine(float eulerAngle)
    {
        if (eulerAngle >= 45f && eulerAngle < 225f)
        {
            return true;
        }
        return false;
    }


    public void changeRegionTo(Region re)
    {
        region = re;
        LoadImage();
    }
    
    private void Flood()
    {
        if (region == Region.Sea && Mathf.Abs(rotateControlInstance.earthAc) > abnormalAc)
        {
            RegionControl[] regionControls = transform.parent.GetComponentsInChildren<RegionControl>();
            int ind = transform.GetSiblingIndex();
            int targetInd;
            if (rotateControlInstance.earthAc > 0)
            {
                if (ind == 0)
                {
                    targetInd = regionControls.Length - 1;
                }
                else
                {
                    targetInd = ind - 1;
                }
            }
            else
            {
                if (ind == regionControls.Length - 1)
                {
                    targetInd = 0;
                }
                else
                {
                    targetInd = ind + 1;
                }
            }
            // 不是海洋变SeaGround
            if (regionControls[targetInd].region != Region.Sea)
            {
                regionControls[targetInd].changeRegionTo(Region.SeaGround);
                regionControls[targetInd].nowEbbTime = ebbTime;
            }
        }
    }

    private void Ebb()
    {
        if (region == Region.SeaGround)
        {
            nowEbbTime -= Time.fixedDeltaTime;
            if (nowEbbTime <= 0f)
            {
                changeRegionTo(Region.FlatGround);
            }
        }
    }
}
