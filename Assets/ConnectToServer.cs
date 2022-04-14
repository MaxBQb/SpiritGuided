using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    void Start() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => SceneManager.LoadScene("Menu");
    
    public void Exit() => GameManager.Exit();
}
