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
    public bool hasYunShiOverCd;
    public float yunShiGaiLv;
    public GameObject yunShi;
    Transform yunShiTrans;
    public float yunShiSpeed;
    public float yunShiMoveTime;
    public float yunShiIntervalTime;
    public Text fixedText;
    public Text secondText;
    public LineRenderer line;
    float yunShiOverTime;
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

    public static Emergency emergencyInstance;
    private void Awake()
    {
        if (emergencyInstance != null)
        {
            Destroy(emergencyInstance);
        }
        emergencyInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        isWarning = false;
        earth = Earth.earth;
        hasEmergency = false;
        hasYunShi = false;
        hasYunShiOverCd = true;
        nowETInterval = 0f;
    }

    private void FixedUpdate()
    {
        if (nowETInterval >= 0f)
        {
            nowETInterval -= Time.fixedDeltaTime;
        }
        if (earth.pol > 0)
        {
            if (!hasEmergency || hasYunShi)
            {
                YunShi();
            }

            if (!hasEmergency || hasET)
            {
                ET();
            }

        }
        if (isWarning)
        {
            nowETWarningTime -= Time.fixedDeltaTime;
            secondText.text = nowETWarningTime.ToString();
        }
    }
    public void YunShi()
    {
        if (!hasYunShi && hasYunShiOverCd)
        {
            float range = Random.value * yunShiGaiLv;
            if (range > 0 && range < 1)
            {
                hasYunShi = true;
                float dir = Random.Range(0, 2 * Mathf.PI);
                Vector3 position = new Vector2(yunShiMoveTime * yunShiSpeed * Mathf.Cos(dir), yunShiSpeed * yunShiMoveTime * Mathf.Sin(dir));
                yunShiTrans = Instantiate(yunShi, position + earth.transform.position, Quaternion.Euler(new Vector3(0, 0, dir / Mathf.PI * 180))).transform;
                yunshiDir = new Vector2(-Mathf.Cos(dir), -Mathf.Sin(dir));
                hasEmergency = true;
                hasYunShiOverCd = false;
                yunShiOverTime = 0;
                line.SetPosition(1, yunShiTrans.position);
                fixedText.text = "陨石警告";
            }
        }
        else if (hasYunShi == true)
        {
            yunShiTrans.Translate(yunshiDir * Time.deltaTime * yunShiSpeed,Space.World);
            Vector2 dis = yunShiTrans.position - earth.transform.position;
            float second = (dis.magnitude - 1.69f) / yunShiSpeed;
            secondText.text = second.ToString() + " s";
        }
        else if (!hasYunShiOverCd)
        {
            yunShiOverTime += Time.deltaTime;
            if (yunShiOverTime >= yunShiIntervalTime)
            {
                hasYunShiOverCd = true;
                yunShiOverTime = 0;
            }
        }
    }
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
                fixedText.text = "UFO即将来袭";
                float dir = Random.Range(0, 2 * Mathf.PI);
                Vector3 position = new Vector3(Mathf.Cos(dir), Mathf.Sin(dir));
                position *= UFO.GetComponent<UFOManager>().UFOHeight * 2;

                // 播放UFO动画

                StartCoroutine(GenerateUFO(position, dir));
            }
        }
    } 


    IEnumerator GenerateUFO(Vector3 position, float dir)
    {
        isWarning = true;
        nowETWarningTime = ETWarningTime;
        yield return new WaitForSeconds(ETWarningTime);
        fixedText.text = "";
        secondText.text = "";
        isWarning = false;
        Instantiate<GameObject>(UFO, position, Quaternion.Euler(0, 0, dir * Mathf.Rad2Deg - 90f));
    }
}
