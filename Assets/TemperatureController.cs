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
        if (Earth.earth.era >= Era.AtomicEra)
        {
            sprite.color = normalColor;
            return;
        }

        if (Mathf.Abs(region.temperature) <= region.warningTemperature)
        {
            sprite.color = normalColor;
        }
        else if(region.temperature > 0f)
        {
            float t = (region.temperature - region.warningTemperature) / (region.maxTemperature - region.warningTemperature);
            sprite.color = Color.Lerp(normalColor, hotColor, t);
        }
        else
        {
            float t = (-region.warningTemperature - region.temperature) / (region.maxTemperature - region.warningTemperature);
            sprite.color = Color.Lerp(normalColor, coldColor, t);
        }
    }
}
