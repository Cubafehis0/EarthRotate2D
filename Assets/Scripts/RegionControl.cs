using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionControl : MonoBehaviour
{
    public float temperatureUpRatio;
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
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        LoadImage();
        temperature = initTemperature;
        earth = Earth.earth;
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
        pol = (int)polF;
        if(pol>earth.maxPol)
        {
            pol = earth.maxPol;
            polF = pol;
        }
    }

    private bool isUnderSunshine(float eulerAngle)
    {
        if (eulerAngle >= 45f && eulerAngle < 225f)
        {
            return true;
        }
        return false;
    }
}
