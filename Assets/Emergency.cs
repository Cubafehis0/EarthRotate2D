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

        earth = Earth.earth;
        hasEmergency = false;
        hasYunShi = false;
        hasYunShiOverCd = true;
    }

    private void FixedUpdate()
    {

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
                yunShiTrans = Instantiate(yunShi, position + earth.transform.position, Quaternion.identity).transform;
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
            yunShiTrans.Translate(yunshiDir * Time.deltaTime * yunShiSpeed);
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
        if (!hasET)
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

                StartCoroutine(GenerateUFO(position));
            }
        }
    } 


    IEnumerator GenerateUFO(Vector3 position)
    {
        yield return new WaitForSeconds(ETWarningTime);
        Instantiate(UFO, position + earth.transform.position, Quaternion.identity);
    }
}
