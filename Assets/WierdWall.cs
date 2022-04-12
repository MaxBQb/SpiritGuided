using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class WierdWall : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        rend.material.DOColor(Color.red, 5f);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        rend.material.DOColor(Color.white, 5f);
    }
}
