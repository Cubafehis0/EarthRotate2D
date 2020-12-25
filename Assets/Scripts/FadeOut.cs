using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 淡出效果组件类。
/// </summary>
public class FadeOut : MonoBehaviour
{
    #region 可视变量
    [HideInInspector] [Tooltip("消失时延。")] public float delaySecond = 1F;
    #endregion

    #region 成员变量
    private SpriteRenderer spriteRenderer = null;
    private float fadeSpeed = 0;    // 消逝速度
    #endregion

    #region 功能方法
    /// <summary>
    /// 第一帧调用之前触发。
    /// </summary>
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a / 2);
        fadeSpeed = this.spriteRenderer.color.a * Time.fixedDeltaTime / delaySecond;
        //StartCoroutine(DestroyNow());
    }

    /*
    /// <summary>
    /// 定时自杀。
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyNow()
    {
        yield return new WaitForSeconds(delaySecond);
        Destroy(gameObject);
    }
    */

    /// <summary>
    /// 降低对象透明度，为0后摧毁对象。
    /// 在固定物理帧刷新时触发。
    /// </summary>
    private void FixedUpdate()
    {
        float alpha = spriteRenderer.color.a - fadeSpeed;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        if (alpha <= 0)
            Destroy(gameObject);
    }
    #endregion
}