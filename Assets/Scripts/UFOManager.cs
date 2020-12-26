using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOManager : MonoBehaviour
{
    public float UFOHeight = 2f;
    public int HP = 20;
    public float RoundSpeed = 2f;
    public GameObject Raser;
    public Transform RaserTrans;

    bool hasShoot = false;
    GameObject raser;
    public float shootTime = 0.5f;
    public float RoundTime = 2f;
    public float FocusTime = 10f;
    private float nowMoveTime;
    public float FocusDecresePercent = 0.05f;

    bool isFocusing;
    float focusingTime;
    Vector3 desPos;
    float moveSpeed = 1f;
    bool initMove = false;
    UFOMoveType moveType;
    Emergency EmergencyInstance;
    public float animationTime = 2f;

    public GameObject RaserAnim;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        EmergencyInstance = Emergency.emergency;
        desPos = transform.position.normalized * UFOHeight;
        initMove = true;
        nowMoveTime = RoundTime;
        moveType = UFOMoveType.Round;
        anim = RaserAnim.GetComponent<Animator>();
        focusingTime = 0f;
        isFocusing = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (initMove)
        {
            initMoveTo(desPos);
        }
        else
        {
            nowMoveTime -= Time.fixedDeltaTime;
            if (nowMoveTime < 0f)
            {
                if (moveType == UFOMoveType.Round)
                {
                    float rand = Random.value;
                    if (rand < 0.2f)
                    {
                        moveType = UFOMoveType.Focus;
                        nowMoveTime = FocusTime;
                    }
                    else
                    {
                        moveType = UFOMoveType.Shoot;
                        nowMoveTime = shootTime;
                        hasShoot = false;
                    }
                }
                else
                {
                    if (moveType == UFOMoveType.Focus)
                    {
                        DeFocus();
                    }
                    moveType = UFOMoveType.Round;
                    nowMoveTime = RoundTime;
                }

            }
            Move(moveType);
        }

        if (HP <= 0)
        {
            OnUFOCrash();
        }
        // test crash
        if (Input.touchCount >= 2)
        {
            Debug.Log("crash");
            OnUFOCrash();
        }
    }

    void initMoveTo(Vector3 destination)
    {
        if (Vector3.Distance(transform.position, destination) <= 0.1f)
        {
            initMove = false;
            return;
        }
        transform.position = Vector2.Lerp(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    private void Move(UFOMoveType type)
    {
        switch (type)
        {
            case UFOMoveType.Round:
                transform.Rotate(Vector3.forward, RoundSpeed * Time.deltaTime, Space.World);
                float theta = transform.rotation.eulerAngles.z;
                transform.position = new Vector3(-UFOHeight * Mathf.Sin(theta * Mathf.Deg2Rad), UFOHeight * Mathf.Cos(theta * Mathf.Deg2Rad), 0f);
                break;
            case UFOMoveType.Focus:
                Focus();
                break;
            case UFOMoveType.Shoot:
                // 发射激光
                if (!hasShoot)
                {
                    raser = Instantiate<GameObject>(Raser, RaserTrans.position, RaserTrans.rotation);
                    hasShoot = true;
                }
                break;
            default:
                break;
        }
    }

    void Focus()
    {
        //transform.parent = focusCity.transform;
        //transform.localPosition = new Vector3(0f, transform.localPosition.y, transform.localPosition.z);
        //transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        //RegionControl regionController = focusCity.GetComponent<RegionControl>();
        //focusingTime += Time.fixedDeltaTime;
        //if (focusingTime >= 3f)
        //{
        //    regionController.polF -= (regionController.pol * FocusDecresePercent);
        //}
        //else if (focusingTime >= FocusTime)
        //{
        //    regionController.polF = 0f;
        //}

        // 释放激光
        if (!RaserAnim.activeSelf)
        {
            RaserAnim.SetActive(true);
        }
        if (!isFocusing)
        {
            StartCoroutine(Idle(anim));
        }
        isFocusing = true;
        focusingTime += Time.fixedDeltaTime;
        if (focusingTime >= 0.1f)
        {
            focusingTime = 0f;
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, Vector3.zero - transform.position, UFOHeight, 1 << LayerMask.NameToLayer("Region"));
            if (hit.collider != null)
            {
                RegionControl region = hit.collider.gameObject.GetComponent<RegionControl>();
                if (region != null)
                {
                    region.polF -= region.polF * FocusDecresePercent;
                }
            }
        }
        
    }

    void DeFocus()
    {
        //Debug.Log("Defocus");
        //transform.parent = null;
        //focusCity = null;
        // 停止激光
        isFocusing = false;
        anim.SetTrigger("end");
        StartCoroutine(DelaySetDeactive(RaserAnim));
    }

    IEnumerator Idle(Animator anim)
    {
        yield return new WaitForSeconds(.9f);
        anim.SetTrigger("idle");
    }

    IEnumerator DelaySetDeactive(GameObject gameObject)
    {
        yield return new WaitForSeconds(.9f);
        gameObject.SetActive(false);
    }

    void OnUFOCrash()
    {
        // UFO破坏动画
        gameObject.GetComponent<Crasher>().Crash();
        // 改变时代
        Earth.earth.hasFinishEraTask[1] = true;
        // 改变emergency状态
        EmergencyInstance.hasEmergency = false;
        EmergencyInstance.hasET = false;
        EmergencyInstance.nowETInterval = EmergencyInstance.ETInterval;

        //  收炮
        foreach (var region in Earth.earth.regionControls)
        {
            if (region.nowAAG != null && region.nowAAG.activeSelf)
            {
                region.nowAAG.GetComponent<Animator>().SetTrigger("end");
                StartCoroutine(SetDeactiveLater(region.nowAAG));
            }
        }
        // Destroy
        Destroy(gameObject);
    }
    IEnumerator SetDeactiveLater(GameObject AAG)
    {
        yield return new WaitForSeconds(1.1f);
        AAG.SetActive(false);
    }
}
