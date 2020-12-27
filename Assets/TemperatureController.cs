using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureController : MonoBehaviour
{
    RegionControl region;
    public  Color hotColor;
    public Color coldColor;
    public Color normalColor;
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        region = GetComponentInParent<RegionControl>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(region.temperature) <= region.abnormalTemperature)
        {
            sprite.color = normalColor;
        }
        else if(region.temperature > 0f)
        {
            float t = (region.temperature - region.abnormalTemperature) / (region.maxTemperature - region.abnormalTemperature);
            sprite.color = Color.Lerp(normalColor, hotColor, t);
        }
        else
        {
            float t = (-region.abnormalTemperature - region.temperature) / (region.maxTemperature - region.abnormalTemperature);
            sprite.color = Color.Lerp(normalColor, coldColor, t);
        }
    }
}
