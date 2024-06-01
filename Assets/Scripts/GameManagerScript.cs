using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverUI;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Ded()
    {
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
    }
    public void TnxForAd()
    {
        gameOverUI.SetActive(false);
        Time.timeScale = 1;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
