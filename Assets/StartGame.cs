using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StartGame : MonoBehaviour
{
    SpriteRenderer[] renderers;
    public int level;
    public float waitTimes;
    float time;
    bool hasAnimation;
    private void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        Debug.Log(renderers.Length);
        hasAnimation = false;
    }
    private void FixedUpdate()
    {
        if(hasAnimation)
        {
            time += Time.deltaTime;
            foreach(var renderer in renderers)
            {
                Color color = renderer.color;
                renderer.color = new Color(color.r, color.g, color.b, 1 - time / waitTimes);
            }
        }
    }
    private void Update()
    {
        if(Input.anyKeyDown)
        {
            StartCoroutine(Animation());
            hasAnimation = true;
        }
    }
    IEnumerator Animation()
    {
        yield return new WaitForSeconds(waitTimes);
        SceneManager.LoadScene(level);
    }
}
