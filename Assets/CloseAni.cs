using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CloseAni : MonoBehaviour
{
    public float waitTime;
    float time;
    Image[] images;
    bool hasAnimation;
    public void Awake()
    {
        //renderers =FindObjectsOfType<SpriteRenderer>();
        //images = FindObjectsOfType<Image>();
        images = GetComponentsInChildren<Image>();
        //hasAnimation = true;
        //StartCoroutine(Show());

    }
    void FixedUpdate()
    {
        if (hasAnimation)
        {
            time += Time.deltaTime;
            if(time>waitTime)
            {
                time = 0;
                hasAnimation = false;
                gameObject.SetActive(false);
            }
            //foreach (var renderer in renderers)
            //{
            //    Color color = renderer.color;
            //    renderer.color = new Color(color.r, color.g, color.b, time / waitTime);
            //}
            foreach (var image in images)
            {
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, 1-time / waitTime);
            }
        }
    }
    public void Close()
    {
        hasAnimation = true;
    }
    IEnumerator Show()
    {
        yield return new WaitForSeconds(waitTime+0.1f);
        hasAnimation = false;
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
