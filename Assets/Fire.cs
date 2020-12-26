using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float fireCD = 0.7f;
    float nowCD;
    float readyTime = 1f;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        nowCD = 0f;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (readyTime >= 0f)
        {
            readyTime -= Time.fixedDeltaTime;
            return;
        }
        
        if (!Emergency.emergencyInstance.hasET)
        {
            return;
        }
        if (nowCD <= 0f)
        {
            anim.SetTrigger("Fire");
            Instantiate<GameObject>(bullet, bulletPos.position, bulletPos.rotation);
            nowCD = fireCD;
        }
        else
        {
            nowCD -= Time.fixedDeltaTime;
        }
    }
}
