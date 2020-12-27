using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionSprite : MonoBehaviour
{
    public static RegionSprite regionSprite;
    private void Awake()
    {
        if(regionSprite!=null)
        {
            Destroy(regionSprite);
        }
        regionSprite = this;
    }
    /// <summary>
    /// 城市的警铃前四个按照顺序摆放，第5个位工业时代的另一种形态
    /// </summary>
    public Sprite[] citySprites; 
    public Sprite sea;
    public Sprite seaGround;
    public Sprite flatground;
    public Sprite desert;
    public Sprite forest;
    public Sprite ironGround;
    public Sprite motor;
}
