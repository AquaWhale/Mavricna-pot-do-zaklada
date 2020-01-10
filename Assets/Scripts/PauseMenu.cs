using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;

    //Objekt pause menu v UI
    public GameObject pauseMenu;
    //Camera background music
    public AudioSource backgroundMusic;
    public AudioSource pauseMenuBackgroundMusic;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (!GameState.deadByCollision && !GameState.deadByExplosion))
        {
            //Ce je igra ze pavzirana potem jo nadaljujemo, ce ne jo pavziramo
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        //skrijemo pause menu
        pauseMenu.SetActive(false);
        pauseMenuBackgroundMusic.Stop();
        backgroundMusic.Play();
        //ponovno zazenemo premikanje casa v igri
        Time.timeScale = 1.0f;
        //igra ni pavzirana vec
        paused = false;
    }

    public void PauseGame()
    {
        //prikazemo pause menu
        pauseMenu.SetActive(true);
        backgroundMusic.Pause();
        pauseMenuBackgroundMusic.Play();
        //ustavimo premikanje casa v igri
        Time.timeScale = 0.0f;
        //zabelezimo si, da je igra pavzirana
        paused = true;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        //Nastavimo da time spet teče, da poteka animacija v main menu
        Time.timeScale = 1.0f;
        paused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void restartGame()
    {
        //Ponastavimo game state in ponastavimo vse vrednosti prejšnje igre
        GameState.restartGame();
        //Ponovno naloadamo igro in ponastavimo parametre
        SceneManager.LoadScene("Game");
        Time.timeScale = 1.0f;
        paused = false;
    }
}
