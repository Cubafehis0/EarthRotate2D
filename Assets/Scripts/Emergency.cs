using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Emergency : MonoBehaviour
{

    Earth earth;
    // yunshi
    public bool hasEmergency;
    public bool hasYunShi;

    public GameObject yunShi;
    Transform yunShiTrans;
    public float yunShiSpeed;
    public float yunShiMoveTime;
    float yunShiNeedTime;
    float yunShiTimeSinceLast;


    int[] yunShiOccurTime = { 20, 10, 10, 5 };
    Vector2 yunshiDir;

    // ET
    public bool hasET;
    public float ETProbability;
    public float ETWarningTime;
    private float nowETWarningTime;
    public GameObject UFO;
    public float ETInterval;
    public float nowETInterval;
    private bool isWarning;
    public GameObject defenseBarrier;
    public static Emergency emergency;
    private void Awake()
    {
        if (emergency != null)
        {
            Destroy(emergency);
        }
        emergency = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        isWarning = false;
        earth = Earth.earth;
        hasEmergency = false;
        hasYunShi = false;
        nowETInterval = 0f;
        yunShiTimeSinceLast = 0;
    }

    private void FixedUpdate()
    {
        if (nowETInterval >= 0f)
        {
            nowETInterval -= Time.fixedDeltaTime;
        }
        if (earth.pol > 0)
        {
            //if (!hasEmergency || hasYunShi)
            //{
            //    YunShi();
            //}
            YunShi();
            if (!hasEmergency || hasET)
            {
                ET();
            }

        }
        if (isWarning)
        {
            nowETWarningTime -= Time.fixedDeltaTime;
        }
        if (hasET)
        {
            foreach (var region in earth.regionControls)
            {
                if (region.nowAAG != null && !region.nowAAG.activeSelf)
                {
                    region.nowAAG.SetActive(true);
                }
            }
        }
        else
        {
            foreach (var region in earth.regionControls)
            {
                if (region.nowAAG != null && region.nowAAG.activeSelf)
                {
                    region.nowAAG.GetComponent<Animator>().SetTrigger("end");
                    StartCoroutine(SetDeactiveLater(region.nowAAG));
                }
            }
        }
    }

    IEnumerator SetDeactiveLater(GameObject AAG)
    {
        yield return new WaitForSeconds(1f);
        AAG.SetActive(false);
    }

    public void YunShi()
    {
        yunShiTimeSinceLast += Time.deltaTime;
        if(yunShiTimeSinceLast>yunShiNeedTime)
        {
            yunShiTimeSinceLast = 0;
            hasYunShi = true;
            float dir = Random.Range(0, 2 * Mathf.PI);
            Vector3 position = new Vector2(yunShiMoveTime * yunShiSpeed * Mathf.Cos(dir), yunShiSpeed * yunShiMoveTime * Mathf.Sin(dir));
            yunShiTrans = Instantiate(yunShi, position + earth.transform.position, Quaternion.Euler(new Vector3(0, 0, dir / Mathf.PI * 180))).transform;
            yunshiDir = new Vector2(-Mathf.Cos(dir), -Mathf.Sin(dir));
        }
        else if (hasYunShi == true)
        {
            yunShiTrans.Translate(yunshiDir * Time.deltaTime * yunShiSpeed, Space.World);
        }
    }
    //public void YunShi()
    //{
    //    if (!hasYunShi && hasYunShiOverCd)
    //    {
    //        float range = Random.value * yunShiGaiLv;
    //        if (range > 0 && range < 1)
    //        {
    //            hasYunShi = true;
    //            float dir = Random.Range(0, 2 * Mathf.PI);
    //            Vector3 position = new Vector2(yunShiMoveTime * yunShiSpeed * Mathf.Cos(dir), yunShiSpeed * yunShiMoveTime * Mathf.Sin(dir));
    //            yunShiTrans = Instantiate(yunShi, position + earth.transform.position, Quaternion.Euler(new Vector3(0, 0, dir / Mathf.PI * 180))).transform;
    //            yunshiDir = new Vector2(-Mathf.Cos(dir), -Mathf.Sin(dir));
    //            hasEmergency = true;
    //            hasYunShiOverCd = false;
    //            yunShiOverTime = 0;
    //            line.SetPosition(1, yunShiTrans.position);
    //            fixedText.text = "陨石警告";
    //            if(earth.era==Era.AtomicEra)
    //                defenseBarrier.SetActive(true);
    //        }
    //    }
    //    else if (hasYunShi == true)
    //    {
    //        yunShiTrans.Translate(yunshiDir * Time.deltaTime * yunShiSpeed,Space.World);
    //        Vector2 dis = yunShiTrans.position - earth.transform.position;
    //        float second = (dis.magnitude - 1.69f) / yunShiSpeed;
    //        secondText.text = second.ToString() + " s";
    //    }
    //    else if (!hasYunShiOverCd)
    //    {
    //        yunShiOverTime += Time.deltaTime;
    //        if (yunShiOverTime >= yunShiIntervalTime)
    //        {
    //            hasYunShiOverCd = true;
    //            yunShiOverTime = 0;
    //        }
    //    }
    //}
    // Update is called once per frame
    void Update()
    {

    }

    private void ET()
    {
        if (!hasET && nowETInterval <= 0f)
        {
            float range = Random.value * ETProbability;
            if (range > 0 && range < 1)
            {
                hasET = true;
                hasEmergency = true;
                float dir = Random.Range(0, 2 * Mathf.PI);
                Vector3 position = new Vector3(Mathf.Cos(dir), Mathf.Sin(dir));
                position *= UFO.GetComponent<UFOManager>().UFOHeight * 2;

                // 播放UFO动画

                StartCoroutine(GenerateUFO(position, dir));
            }
        }
    } 

    public void ChangYunShiGaiLv(Era era)
    {
        int level = (int)era;
        yunShiNeedTime = yunShiOccurTime[level];
    }
    IEnumerator GenerateUFO(Vector3 position, float dir)
    {
        isWarning = true;
        nowETWarningTime = ETWarningTime;
        yield return new WaitForSeconds(ETWarningTime);
        isWarning = false;
        Instantiate<GameObject>(UFO, position, Quaternion.Euler(0, 0, dir * Mathf.Rad2Deg - 90f));
    }
}
