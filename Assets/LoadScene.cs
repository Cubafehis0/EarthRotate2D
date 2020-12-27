using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour
{
    public int level;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ClickToLoadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
