using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockBullet : MonoBehaviour
{
    GameObject UFO;
    public float smooth = 5f;
    public int attackNum = 10;
    // Start is called before the first frame update
    void Start()
    {
        UFO = FindObjectOfType<UFOManager>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, UFO.transform.position, smooth * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.tag == "UFO")
        {
            UFOManager ufo = collision.collider.gameObject.GetComponent<UFOManager>();
            ufo.HP -= attackNum;
            ufo.Attacked();

            Destroy(gameObject);
        }
    }
}
