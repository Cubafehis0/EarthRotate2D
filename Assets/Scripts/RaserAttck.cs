using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaserAttck : MonoBehaviour
{
    public float populationDecreasePercent = 0.1f;
    public float RaserSpeed = 5f;

    private void FixedUpdate()
    {
        Vector3 raserDir = (Vector3.zero - transform.position).normalized;
        transform.Translate(raserDir * RaserSpeed * Time.fixedDeltaTime, Space.World);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.gameObject.layer ==  LayerMask.NameToLayer("Region")) 
        {
            RegionControl regionController = collision.collider.gameObject.GetComponent<RegionControl>();
            regionController.polF -= populationDecreasePercent * regionController.pol;
            // 激光爆炸效果
            Debug.Log("Raser hit " + regionController.transform.name);
            Destroy(gameObject);
        }
    }
}
