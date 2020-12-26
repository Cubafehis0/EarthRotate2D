using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class RotateControl : MonoBehaviour
{
    public GameObject handle;
    public Vector2 lastPos;
    public Vector2 nowPos;
    public Transform centerTrans;
    public float angleSpeed;
    public float radius;
    public float angle;
    public static RotateControl rotateControlInstance;
    [Tooltip("")]
    GameObject earth;
    [Tooltip("handle中心")]
    public Vector2 center;
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
    public bool startGame;
    public ControlMode controlMode;
    public int touchRange;
    //一个变量，作为切换时
    public bool isSwitching;
    Touch touch;

    bool isTouching;

    public float recordGap = 0.2f;
    private float recordTime;
    public string path = @"Assets/";
    StreamWriter sw;
    // Start is called before the first frame update
    private void Awake()
    {

        if (rotateControlInstance != null)
        {
            Destroy(rotateControlInstance);
        }
        rotateControlInstance = this;
    }
    void Start()
    {
        earth = Earth.earth.gameObject;
        startGame = false;
        lastPos = new Vector2(0, 0);
        nowPos = new Vector2(0, 0);
        center = centerTrans.position;
        handle.SetActive(false);
        damp = 0;

        FileInfo fi = new FileInfo(path + "//" + "record.txt");
        sw = fi.CreateText();
        recordTime = recordGap;
    }
    private void FixedUpdate()
    {
        //if(isSwitching)
        //{
        //    //earthAc = 0;
        //    //earthS = 0;
        //    //float earthAngle = Earth.earth.transform.rotation.eulerAngles.z;
        //    //nowPos = center + new Vector2(100 * Mathf.Cos(earthAngle), 100 * Mathf.Sin(earthAngle));
        //    //lastPos = nowPos;
        //    isSwitching = false;
        //}
        if (isTouching)
        {
            float targetSpeed = angleSpeed / scale;
            earthAc = (targetSpeed - earthS) * Time.deltaTime;
        }
        earthAc /= scale;
        HandleMove();
        EarthMove();
        //if (controlMode == ControlMode.Normal)
        //{
        //    EarthMove();
        //}
        //else if (controlMode == ControlMode.Emergency)
        //{
        //    EarthMoveInEmergeny();
        //}
        if (Mathf.Abs(earthS) > 10f)
        {
            startGame = true;
        }

        recordTime -= Time.fixedDeltaTime;
        if (recordTime <= 0f)
        {
            sw.WriteLine(earthS);
            recordTime = recordGap;
        }       
    }

    private void OnDestroy()
    {
        sw.Close();
        sw.Dispose();
    }
    void HandleMove()
    {
        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 pos = nowPos - center;
            if (pos.magnitude < radius)
            {
                handle.transform.position = nowPos;
            }
            else
            {
                Vector2 dir = pos.normalized;
                handle.transform.position = center + radius * dir;
            }
        }
    }
    void EarthMove()
    {
        earthS += earthAc;
        earth.transform.Rotate(0, 0, earthS * Time.deltaTime);
    }
    void EarthMoveInEmergeny()
    {
        float sourAngle = earth.transform.rotation.eulerAngles.z;
        Vector2 des = nowPos - center;
        float targetAngle = Vector2.SignedAngle(new Vector2(1,0), des)+180;
        float targetSpeed = (targetAngle - sourAngle) / Time.deltaTime;
        Debug.Log(targetAngle);
        Debug.Log(sourAngle);
        earthAc = targetSpeed - earthS;
        earthS += earthAc;
        earth.transform.Rotate(0, 0, earthS * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            isTouching = true;
            touch = Input.GetTouch(0);
            if (Vector2.Distance(touch.position, center) < touchRange)
            {
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
                    angle = Vector2.SignedAngle(sour, dis);
                    angleSpeed = angle / Time.deltaTime;
                    lastPos = nowPos;
                    nowPos = touch.position;
                }
                
            }
            if (touch.phase == TouchPhase.Ended)
            {
                handle.SetActive(false);
                lastPos = nowPos;
                angleSpeed = 0;
            }
        }
        else
        {
            isTouching = false;
        }
    }
}
