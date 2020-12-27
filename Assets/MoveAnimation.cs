using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimation : MonoBehaviour
{
    public float moveSpeed;
    Vector2[] dir = { new Vector2(1, 1 ), new Vector2(1, -1),new Vector2(-1, -1),new Vector2(-1, 1) };
    int nowDirIndex;
    Vector2 nowDir;
    float moveTime;
    public float moveTimePerDir;
    // Start is called before the first frame update
    void Start()
    {
        nowDirIndex = 0;
        nowDir = dir[nowDirIndex];
    }
    private void FixedUpdate()
    {
        moveTime += Time.deltaTime;
        if (moveTime > moveTimePerDir)
        {
            moveTime = 0;
            nowDirIndex = (nowDirIndex + 1) % dir.Length;
            nowDir = dir[nowDirIndex];
        }
        float value1 = Random.value;
        float value2 = Random.value;
        transform.Translate(new Vector2(nowDir.x*Time.deltaTime*moveSpeed*value1,nowDir.y*Time.deltaTime*moveSpeed*value2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
