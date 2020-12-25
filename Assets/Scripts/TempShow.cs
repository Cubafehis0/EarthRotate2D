using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TempShow : MonoBehaviour
{
    public RegionControl region;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        text.text = region.temperature.ToString();
    }
}
