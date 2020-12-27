using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 显示提示信息 
/// 0 人类的防御系统击毁了飞碟
/// 1 人类建立了新的城市
/// 2 人类建设了防御武器
/// 3 外星人被击败
/// 4 人类建设了
/// </summary>
public class EventTip : MonoBehaviour
{
    //单例
    public static EventTip eventTip; 

    public Sprite[] tips=null;
    bool[] hasTip=new bool[100];
    public Vector2 BeginPos;
    public float tipinterval;
    public Transform tipParent;
    public List<SpriteRenderer> tipShows=new List<SpriteRenderer>();
    //所有提示框的数目
    public int tipCnt;
    //有信息提示的提示框的数目
    int hasTipShowCnt;
    //事件出现事件
    public float waitSecond;
    //每个tip的计时器
    float[] waitTimes = new float[1000];
    private void Awake()
    {
        if(eventTip!=null)
        {
            Destroy(eventTip);
        }
        eventTip = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tipCnt; i++)
        {
            GameObject spriteShow = new GameObject("tipShow_" + i.ToString());
            spriteShow.transform.parent = tipParent;
            SpriteRenderer renderer=spriteShow.AddComponent<SpriteRenderer>();
            spriteShow.transform.position = BeginPos - new Vector2(0, tipinterval * i);
            renderer.sprite = null;
            tipShows.Add(renderer);
        }
        hasTipShowCnt = 0;
    }
    private void FixedUpdate()
    {
        Show();
    }
    void Show()
    {
        //时间到了的tip的数目
        //顺序添加，先进先出
        int overTimeShowCnt=0;
        int notOverTimeShowCnt = 0;
        for (int i = 0; i < hasTipShowCnt; i++)
        {
            waitTimes[i] += Time.deltaTime;
            if(waitTimes[i]>waitSecond)
            {
                overTimeShowCnt++;
            }
            else
            {
                notOverTimeShowCnt++;
            }
        }
        //同一种提示只能出现一次
        if(overTimeShowCnt>0)
        {
            Debug.Log(overTimeShowCnt);
            for (int i = 0; i < overTimeShowCnt; i++)
            {
                int tipIndex = Find(tipShows[i].sprite);
                if (tipIndex >= 0)
                {
                    hasTip[tipIndex] = false;
                }
                tipShows[i].sprite = null;
            }
            for (int i = 0; i < notOverTimeShowCnt; i++)
            {
                int index = (i + overTimeShowCnt) % tipCnt;
                if (tipShows[index].sprite != null)
                {
                    tipShows[i].sprite = tipShows[index].sprite;
                    tipShows[index].sprite = null;
                    waitTimes[i] = waitTimes[index];
                    waitTimes[index] = 0;
                }
            }
            hasTipShowCnt -= overTimeShowCnt;
        }
    }
    int Find(Sprite sprite)
    {
        for (int i = 0; i < tips.Length; i++)
        {
            if(sprite==tips[i])
            {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// 添加新的信息事件
    /// </summary>
    /// <param name="tip"></param>
    public void AddTips(Tip tip)
    {
        if(!hasTip[(int)tip])
        {
            hasTip[(int)tip] = true;
            SpriteRenderer renderer = null;
            //所用渲染器的索引
            int index = 0;
            for (int i = 0; i < tipShows.Count; i++)
            {
                if (tipShows[i].sprite == null)
                {
                    index = i;
                    renderer = tipShows[i];
                    break;
                }
            }
            //没有找到就新建一个
            if (renderer == null)
            {
                index = tipCnt;
                GameObject spriteShow = new GameObject("tipShow_" + tipCnt.ToString());
                spriteShow.transform.parent = tipParent;
                SpriteRenderer newRenderer = spriteShow.AddComponent<SpriteRenderer>();
                spriteShow.transform.position = BeginPos - new Vector2(0, tipinterval * tipCnt);
                newRenderer.sprite = null;
                tipShows.Add(newRenderer);
                tipCnt++;
                renderer = newRenderer;
            }
            waitTimes[index] = 0;
            hasTipShowCnt++;
            renderer.sprite = tips[(int)tip];
        }
    }
    IEnumerator ShowTips(Tip tip)
    {
        SpriteRenderer renderer=null;
        foreach(var render in tipShows)
        {
            if(render.sprite==null)
            {
                renderer = render;
                break;
            }
        }
        renderer.sprite = tips[(int)tip] ;
        yield return new WaitForSeconds(waitSecond);
        renderer.sprite = null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
