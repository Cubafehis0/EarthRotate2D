using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackBullet : MonoBehaviour
{
    public float bulletSpeed = 1f;
    public int AttckNum = 10;
    public float bulletReadyTime = 1.5f;
    float readyTime;
    UFOManager ufomanager;
    Transform UFOTrans;
    private void Start()
    {
        ufomanager = FindObjectOfType<UFOManager>();
        UFOTrans = ufomanager.gameObject.transform;
        readyTime = bulletReadyTime;
    }
    private void FixedUpdate()
    {
        if (readyTime > 0f)
        {
            readyTime -= Time.fixedDeltaTime;
            return;
        }
        if (UFOTrans != null)
        {
            transform.position = Vector3.Lerp(transform.position, UFOTrans.position, bulletSpeed * Time.fixedDeltaTime);
        }
        else
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null)
        {
            if (collision.collider.tag == "UFO")
            {
                UFOManager ufo = collision.collider.GetComponent<UFOManager>();
                ufo.HP -= AttckNum;
                // 子弹击中动画
                ufo.Attacked();


                Destroy(gameObject.transform.parent.gameObject);
            }
            else if (collision.collider.tag == "Bounds")
            {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
}
