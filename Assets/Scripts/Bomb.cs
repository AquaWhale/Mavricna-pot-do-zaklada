using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect;
    private float explosionEffectDuration = 3.0f;
    private void OnTriggerEnter(Collider other)
    {
        //ce se player zaleti vanj
        if (other.gameObject.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
            SoundManager.PlaySound("explosion");
            //naredi effect pobranega
            GameObject effect = Instantiate(explosionEffect) as GameObject;
            effect.transform.position = new Vector3(other.transform.position.x, other.transform.position.y+3f, other.transform.position.z);
            effect.transform.localScale = new Vector3(9.0f, 9.0f, 9.0f);
            GameState.deadByExplosion = true;
            Destroy(effect, explosionEffectDuration);

        }
    }
}
