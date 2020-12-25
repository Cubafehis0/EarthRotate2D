using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOManager : MonoBehaviour
{
    public float UFOHeight = 2f;
    public float moveGap = 2f;
    public float focusTime = 10f;
    float nowMoveTime;
    public int HP = 20;
    public float RoundSpeed = 2f;
    public Transform EarthTransform;

    GameObject focusCity;
    Vector3 desPos;
    float moveSpeed = 1f;
    bool initMove = false;
    UFOMoveType moveType;
    // Start is called before the first frame update
    void Start()
    {
        desPos = transform.position.normalized * UFOHeight;
        initMove = true;
        nowMoveTime = moveGap;
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
            if (nowMoveTime <= 0f)
            {
                nowMoveTime = moveGap;
                float rand = Random.value * 2;
                if (rand < 1f)
                {
                    moveType = UFOMoveType.Focus;
                }
                else
                {
                    moveType = UFOMoveType.Round;
                }
                Move(moveType);
            }
        }
    }

    void initMoveTo(Vector3 destination)
    {
        if (transform.position == destination)
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
                // 
                Vector3 axis = Vector3.forward;
                transform.Rotate(axis, RoundSpeed * Time.fixedDeltaTime, Space.World);
                break;
            case UFOMoveType.Focus:

                break;
            default:
                break;
        }
    }
}
