using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenUI : MonoBehaviour
{
    public GameObject UI;

    public void ClickToOpenUI()
    {
        Time.timeScale = 0;
        UI.SetActive(true);
    }
    public void ClickToCloseUI()
    {
        Time.timeScale = 1;
        UI.SetActive(false);
    }

}
