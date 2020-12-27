using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreChange : MonoBehaviour
{
    public Color ZeroSpeedColor;
    public Color MaxSpeedColor;
    public float maxSpeed = 40f;
    SpriteRenderer sprite;
    RotateControl rotate;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rotate = RotateControl.rotateControlInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rotate.earthS) < maxSpeed)
        {
            sprite.color = Color.Lerp(ZeroSpeedColor, MaxSpeedColor, 1 - (maxSpeed - Mathf.Abs(rotate.earthS)) / maxSpeed);
        }
        else
        {
            sprite.color = MaxSpeedColor;
        }
    }
}
