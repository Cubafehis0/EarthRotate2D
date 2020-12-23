using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionControl : MonoBehaviour
{
    public Region region;
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        LoadImage();
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
}
