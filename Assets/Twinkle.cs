using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twinkle : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CloseTwinkle());
    }
    IEnumerator CloseTwinkle()
    {
        yield return new WaitForSeconds(time);
        Animator animation=GetComponent<Animator>();
        animation.enabled = false;
    }
}
