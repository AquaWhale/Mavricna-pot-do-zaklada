using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject instructionsMenu;
    public GameObject mainMenu;
    public void startGame()
    {
        //Ponastavimo game state na začetni in zbrišemo vse parametre prejšnje igre
        GameState.restartGame();
        SceneManager.LoadScene("Game");
    }

    public void ShowInstructions()
    {
        mainMenu.SetActive(false);
        instructionsMenu.SetActive(true);
    }

    public void HideInstructions()
    {
        instructionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void Start()
    {
        instructionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
