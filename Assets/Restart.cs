using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Restart : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClickToRestart()
    {
        int level = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(level);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
