using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image background;
    private AudioSource mouseOverAudio;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool isActive = tabGroup.OnTabEnter(this);
        if (!isActive)
        {
            mouseOverAudio.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<Image>();
        background.color = tabGroup.tabIdleColor;
        tabGroup.Subscribe(this);
        mouseOverAudio = GetComponent<AudioSource>();
        //selectamo prvi tab
        int index = this.transform.GetSiblingIndex();
        if (index == 0)
        {
            tabGroup.OnTabSelected(this);
        }
    }
}
