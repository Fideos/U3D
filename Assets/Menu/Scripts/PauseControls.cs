using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControls : MonoBehaviour {

    [SerializeField]
    private bool gamePaused;

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
    }

}
