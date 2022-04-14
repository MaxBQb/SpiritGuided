using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using WebSocketSharp;

public class WierdWall : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerExitHandler,
    IPunObservable
{
    public Renderer rend;
    private bool isObservedBySomeone = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        rend.material.DOColor(isObservedBySomeone ? Color.red : Color.white, 5f);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        isObservedBySomeone = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isObservedBySomeone = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isObservedBySomeone);
        }
        else
        {
            isObservedBySomeone = (bool) stream.ReceiveNext();
        }
    }
}
