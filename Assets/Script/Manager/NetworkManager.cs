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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(this.gameObject);
        }
    }

	public override void OnEnable()
	{
		base.OnEnable();	//PhotonNetwork.AddCallbackTarget(this); 메서드로 Callback 받을 대상 지정
		PhotonNetwork.AutomaticallySyncScene = true;
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

		// 네트워크 뷰의 업데이트 빈도를 조절
		PhotonNetwork.SendRate = 20; // 초당 전송할 업데이트 수
		PhotonNetwork.SerializationRate = 10; // 초당 시리얼화할 업데이트 수
	}

    public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		PhotonNetwork.LoadLevel("MainScene");
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (otherPlayer == PhotonNetwork.MasterClient)
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

			foreach (GameObject p in players)
			{
				if (p.GetComponent<PhotonView>().IsMine == true)
				{
					PhotonNetwork.Destroy(p);
				}
			}

			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LoadLevel("MainScene");
		}
	}
}
