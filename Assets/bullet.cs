using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float bulletSpeed = 1f;
    public int AttckNum = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.up * bulletSpeed, Space.Self);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null)
        {
            if (collision.collider.tag == "UFO")
            {
                collision.collider.GetComponent<UFOManager>().HP -= AttckNum;
                // 子弹击中动画

                Destroy(gameObject);
            }
            else if (collision.collider.tag == "Bounds")
            {
                Destroy(gameObject);
            }
        }
    }
}
