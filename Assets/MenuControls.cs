using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class MenuControls : MonoBehaviourPunCallbacks
{
    [SerializeReference] private InputField code;
    [SerializeReference] private Button refresh;
    [SerializeReference] private TextMeshProUGUI error;
    
    private readonly RoomOptions options = new RoomOptions
    {
        MaxPlayers = 10
    };
    
    public void Play() => PhotonNetwork.JoinOrCreateRoom(
        RoomCode.Value, 
        options,
        TypedLobby.Default
    );

    public void Join()
    {
        if (code.text.IsNullOrEmpty())
            PhotonNetwork.JoinRandomRoom();
        else
            PhotonNetwork.JoinRoom(code.text);
    }
    public void Refresh() => RoomCode.Update();
    public void Exit() => GameManager.Exit();

    public void Paste() => code.text = GUIUtility.systemCopyBuffer;

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
        error.text = "";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Join room failed: {message}");
        error.text = "Подключиться не вышло!";
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log($"Join random room failed: {message}");
        error.text = "Похоже, никто ещё не играет, станешь первым?";
    }

    public override void OnCreatedRoom() => refresh.interactable = false;
}
