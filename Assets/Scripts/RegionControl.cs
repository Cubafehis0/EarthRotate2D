using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionControl : MonoBehaviour
{
    public float temperatureUpRatio;
    public float initTemperature;
    public float temperature;
    public Region region;
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        LoadImage();
        temperature = initTemperature;
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
        }
        else
        {
            temperature -= temperatureUpRatio * Time.fixedDeltaTime;
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
