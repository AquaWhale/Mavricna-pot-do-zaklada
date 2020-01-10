using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioClip coinPickupSound, powerUpPickupSound, moveSound, jumpSound, rocketJumpSound, screamSound, crashSound, explosionSound, highScoreReachedSound, splashSound, reversePickupSound;
    // Start is called before the first frame update
    static AudioSource audioSource;
    static string lastSoundPlayed = "";

    void Start()
    {
        coinPickupSound = Resources.Load<AudioClip>("CoinPickup");
        powerUpPickupSound = Resources.Load<AudioClip>("PowerUpPickup");
        moveSound = Resources.Load<AudioClip>("RunDodge");
        jumpSound = Resources.Load<AudioClip>("RunJump");
        rocketJumpSound = Resources.Load<AudioClip>("RocketJump");
        screamSound = Resources.Load<AudioClip>("Scream");
        crashSound = Resources.Load<AudioClip>("Crash");
        explosionSound = Resources.Load<AudioClip>("Explosion");
        highScoreReachedSound = Resources.Load<AudioClip>("HighScoreReached");
        reversePickupSound = Resources.Load<AudioClip>("ReversePickup");
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        //Debug.Log("in Switch");
        switch (clip)
        {
            case "coinPickup":
                lastSoundPlayed = "coinPickup";
                audioSource.PlayOneShot(coinPickupSound, 0.05f);
                break;
            case "powerUpPickup":
                lastSoundPlayed = "powerUpPickup";
                audioSource.PlayOneShot(powerUpPickupSound, 0.2f);
                break;
            case "move":
                lastSoundPlayed = "move";
                audioSource.PlayOneShot(moveSound, 0.2f);
                break;
            case "jump":
                lastSoundPlayed = "jump";
                audioSource.PlayOneShot(jumpSound, 0.3f);
                break;
            case "rocketJump":
                lastSoundPlayed = "rocketJump";
                audioSource.PlayOneShot(rocketJumpSound, 0.3f);
                break;
            case "scream":
                if (!lastSoundPlayed.Equals("scream"))
                {
                    audioSource.PlayOneShot(screamSound, 0.5f);
                    lastSoundPlayed = "scream";
                }
                break;
            case "crash":
                if (!lastSoundPlayed.Equals("crash"))
                {
                    audioSource.PlayOneShot(crashSound, 0.5f);
                    lastSoundPlayed = "crash";
                }
                break;
            case "explosion":
                lastSoundPlayed = "explosion";
                audioSource.PlayOneShot(explosionSound, 0.5f);
                break;
            case "highScoreReached":
                lastSoundPlayed = "highScoreReached";
                audioSource.PlayOneShot(highScoreReachedSound, 0.4f);
                break;
            case "reversePickup":
                lastSoundPlayed = "reversePickup";
                audioSource.PlayOneShot(reversePickupSound, 0.2f);
                break;
        }
    }
}
