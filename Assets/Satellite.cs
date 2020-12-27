using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{
    public float speed;
    public float targetY;
    public GameObject Barrier;
    bool isOpenBarrier;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        if(transform.localPosition.y>targetY)
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0), Space.Self);
        }
        else
        {
            OpenShield();
        }
    }
    private void OpenShield()
    {
        if(!isOpenBarrier)
        {
            Instantiate(Barrier,Earth.earth.transform);
            isOpenBarrier = true;
            EventTip.eventTip.AddTips(Tip.DefenseMeteorite);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
