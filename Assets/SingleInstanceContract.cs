using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SingleInstanceContract : MonoBehaviour
{
    [SerializeReference] private List<GameObject> enableThis;

    void Start()
    {
        enabled = GetComponent<PhotonView>().IsMine;
        foreach (var element in enableThis)
            element.SetActive(enabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
