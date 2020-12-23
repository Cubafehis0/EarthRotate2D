using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateControl : MonoBehaviour
{
    public GameObject handle;
    public Vector2 lastPos;
    public Vector2 nowPos;
    public Transform centerTrans;
    public float angleSpeed;
    public float radius;
    public float angle;
    public static RotateControl rotateControl;
    [Tooltip("")]
    public GameObject Earth;
    [Tooltip("handle中心")]
    Vector2 center;
    [Tooltip("地球角加速度")]
    public float earthAc;
    [Tooltip("地球角速度")]
    public float earthS;
    [Tooltip("静默擦力大小")]
    public float staticDampAbs;
    [Tooltip("动摩擦力大小")]
    public float dynamicDampAbs;
    [Tooltip("加速度上限")]
    public float speedLijie;
    [Tooltip("两个速度之间的映射关系")]
    public float scale;
    public float maxAcc;
    public float maxS;
    public float damp;
    Touch touch;

    bool _beginTouch = false;//Update之外
    // Start is called before the first frame update
    private void Awake()
    {

        if (rotateControl != null)
        {
            Destroy(rotateControl);
        }
        rotateControl = this;

    }
    void Start()
    {
        lastPos = new Vector2(0,0);
        nowPos = new Vector2(0,0);
        center = centerTrans.position;
        handle.SetActive(false);
        damp = 0;
    }
    private void FixedUpdate()
    {
        float targetSpeed = angleSpeed / scale;
        earthAc = (targetSpeed - earthS) / 40;
        //float force = angleSpeed - earthS;

        //阻力和速度成正比
        //float force = angleSpeed;
        //float dampAbs = earthS / maxS * staticDampAbs;
        //if (dampAbs > staticDampAbs)
        //{
        //    dampAbs = staticDampAbs;
        //}
        //if (earthS == 0)
        //{
        //    if (force != 0)
        //    {
        //        damp = -force / Mathf.Abs(force) * dampAbs;
        //        earthAc = force + damp;
        //        if (earthAc * force <= 0)
        //        {
        //            earthAc = 0;
        //        }
        //    }
        //}
        //else
        //{
        //    //if(Mathf.Abs(earthS)<speedLijie)
        //    //{
        //    //    dampAbs = dampAbs;
        //    //}
        //    //else
        //    //{
        //    //    dampAbs = dampAbs;
        //    //}
        //    damp = -earthS / Mathf.Abs(earthS) * dampAbs;
        //    earthAc = force + damp;
        //}


        //if (Mathf.Abs(earthAc)>maxAcc && earthAc*earthS>0)
        //{
        //    earthAc = earthAc / Mathf.Abs(earthAc) * maxAcc;
        //}
        ////防止越界情况
        //if(Mathf.Abs(angleSpeed)<dampAbs && earthS*damp>0)
        //{
        //    earthS = 0;
        //    earthAc = 0;
        //}
        //else if(earthS==0)
        //{
        //    if(angleSpeed!=0)
        //    {
        //        damp = -angleSpeed / Mathf.Abs(angleSpeed) * dampAbs;
        //        earthAc = angleSpeed + damp;
        //        //加速度与阻力在一个方向则不行
        //        if (earthAc*damp >= 0)
        //        {
        //            earthAc = 0;
        //        }
        //    }  
        //    else
        //    {
        //        damp = 0;
        //        earthAc = 0;
        //    }
        //}
        //else
        //{
        //    damp = - earthS / Mathf.Abs(earthS) * Mathf.Abs(damp);
        //    earthAc = angleSpeed + damp;
        //}
        earthAc /= scale;
        HandleMove();
        EarthMove();
    }
    void HandleMove()
    {
        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 pos = nowPos - center;
            if(pos.magnitude<radius)
            {
                handle.transform.position = nowPos;
            }
            else
            {
                Vector2 dir = pos.normalized;
                handle.transform.position = center + radius*dir;
            }
        }
    }
    void EarthMove()
    {
        earthS += earthAc;
        //if(Mathf.Abs(earthAc)<Mathf.Abs(damp) && earthS*damp>0)
        //{
        //    earthS = 0;
        //}
        Earth.transform.Rotate(0,0,earthS * Time.deltaTime);
       
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                nowPos = touch.position;
                lastPos = touch.position;
                angleSpeed = 0;
                handle.SetActive(true);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 sour = lastPos - center;
                Vector2 dis = nowPos - center;
                angle=Vector2.SignedAngle(sour, dis);
                //while(angle>Mathf.PI)
                //{
                //    angle -= 2*Mathf.PI;
                //}
                //while(angle<-Mathf.PI)
                //{
                //    angle += 2*Mathf.PI;
                //}
                angleSpeed = angle / Time.deltaTime;
                lastPos = nowPos;
                nowPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                handle.SetActive(false);
                lastPos = nowPos;
                angleSpeed = 0;
            }
        }
    }
}
