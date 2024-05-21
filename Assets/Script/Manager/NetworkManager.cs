using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance = null;

    LoadBalancingClient client = null;

    private void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	public override void OnEnable()
	{
		base.OnEnable();	//PhotonNetwork.AddCallbackTarget(this); 메서드로 Callback 받을 대상 지정
		Debug.Log("연결 상태 : " + PhotonNetwork.IsConnected);
	}

    private void Update()
	{
		if(client != null)
		{
			client.Service();
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
	}

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("StartScene");
        }

        _ = StartCoroutine(WaitForSceneChange());
    }

    IEnumerator WaitForSceneChange()
    {
        // 현재 Scene이 StartScene이 될 때까지 대기
        while (SceneManager.GetActiveScene().name != "StartScene")
        {
            yield return null;
        }

		// StartScene으로 변경되면 실행할 함수 호출
		PlayerSpawnManager.Instance.MakePlayer(GameObject.Find("SpawnPoint").transform);
	}

    public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		PhotonNetwork.AutomaticallySyncScene = false;
		Destroy(gameObject);
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnLeftLobby()
	{
		base.OnLeftLobby();
		PhotonNetwork.AutomaticallySyncScene = false;
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (otherPlayer.IsMasterClient == true)
		{
			PhotonNetwork.AutomaticallySyncScene = false;

			PhotonNetwork.LeaveRoom();
		}
	}
}
