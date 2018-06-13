using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControls : MonoBehaviour {

    [SerializeField]
    private bool gamePaused;

    public void restartCurrentScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void Awake()
    {
        gamePaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            gamePaused = !gamePaused;
        }

        if (gamePaused)
        {
            PauseGame();
        }
        else
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            restartCurrentScene();
        }
    }

}
