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
    public float RaserSpeed = 5f;

    bool hasShoot = false;
    GameObject raser;
    public float shootTime = 0.5f;
    public float RoundTime = 2f;
    public float FocusTime = 10f;
    private float nowMoveTime;
    public float FocusDecresePercent = 0.05f;

    GameObject focusCity;
    float focusingTime;
    Vector3 desPos;
    float moveSpeed = 1f;
    bool initMove = false;
    UFOMoveType moveType;
    // Start is called before the first frame update
    void Start()
    {
        desPos = transform.position.normalized * UFOHeight;
        initMove = true;
        nowMoveTime = RoundTime;
        moveType = UFOMoveType.Round;
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
                    float rand = Random.value * 2;
                    if (rand < 1f)
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
                RaycastHit2D hit;
                hit = Physics2D.Raycast(transform.position, Vector3.zero - transform.position, UFOHeight);
                if (hit.collider != null)
                {
                    // 如果下方区域没有人口则继续环绕
                    if (hit.collider.gameObject.GetComponent<RegionControl>().pol == 0)
                    {
                        nowMoveTime = 0f;
                        return;
                    }
                    if (focusCity == null)
                    {
                        focusCity = hit.collider.gameObject;
                        focusingTime = 0f;
                    }
                    Focus();
                }
                break;
            case UFOMoveType.Shoot:
                // 发射激光
                if (!hasShoot)
                {
                    raser = Instantiate<GameObject>(Raser, RaserTrans.position, RaserTrans.rotation);
                    hasShoot = true;
                }
                if (raser != null)
                {
                    Vector3 raserDir = (Vector3.zero - raser.transform.position).normalized;
                    raser.transform.Translate(raserDir * RaserSpeed * Time.fixedDeltaTime, Space.World);
                }
                break;
            default:
                break;
        }
    }

    void Focus()
    {
        transform.parent = focusCity.transform;
        transform.localPosition = new Vector3(0f, transform.localPosition.y, transform.localPosition.z);
        transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        RegionControl regionController = focusCity.GetComponent<RegionControl>();
        focusingTime += Time.fixedDeltaTime;
        if (focusingTime >= 3f)
        {
            regionController.polF -= (regionController.pol * FocusDecresePercent);
        }
        else if (focusingTime >= FocusTime)
        {
            regionController.polF = 0f;
        }
    }

    void DeFocus()
    {
        Debug.Log("Defocus");
        transform.parent = null;
        focusCity = null;
    }
}
