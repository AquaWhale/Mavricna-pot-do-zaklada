using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler
{
    private AudioSource mouseOverAudio;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverAudio = GetComponent<AudioSource>();
        mouseOverAudio.Play();
    }

}
