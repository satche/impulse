using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;

    /// <summary>
    /// Pause the game
    /// </summary>
    /// <param name="isPaused">Is the game paused ?</param>
    public void PauseGame(bool isPaused)
    {
        this.isPaused = isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
