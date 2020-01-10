using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public Text finalScoreText;
    public Text gameOverText;
    //Ko umre spremeni glasbo v ozadju
    public AudioSource cameraBackgroundAudio;
    public AudioClip deathBackgroundSound;
    private bool shown = false;

    public void showEndMenu()
    {
        if (shown)
        {
            return;
        }
        shown = true;
        gameObject.SetActive(true);
        cameraBackgroundAudio.clip = deathBackgroundSound;
        cameraBackgroundAudio.Play();
        finalScoreText.text = GameState.coinScore.ToString();
        if (GameState.coinScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", GameState.coinScore);
            PlayerPrefs.Save();
        }
        //Ce je eksplodiral prikazemo drug text
        if (GameState.deadByExplosion)
        {
            gameOverText.text = "Ups, eksplodiral si!\nŠtevilo nabranih kovančkov: ";
        }
    }
    
}
