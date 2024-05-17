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
		base.OnEnable();	//PhotonNetwork.AddCallbackTarget(this); �޼���� Callback ���� ��� ����
		PhotonNetwork.AutomaticallySyncScene = true;
		Debug.Log("���� ���� : " + PhotonNetwork.IsConnected);
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
        // ���� Scene�� StartScene�� �� ������ ���
        while (SceneManager.GetActiveScene().name != "StartScene")
        {
            yield return null;
        }

		// StartScene���� ����Ǹ� ������ �Լ� ȣ��
		PlayerSpawnManager.Instance.MakePlayer(GameObject.Find("SpawnPoint").transform);

		// ��Ʈ��ũ ���� ������Ʈ �󵵸� ����
		PhotonNetwork.SendRate = 20; // �ʴ� ������ ������Ʈ ��
		PhotonNetwork.SerializationRate = 10; // �ʴ� �ø���ȭ�� ������Ʈ ��
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
