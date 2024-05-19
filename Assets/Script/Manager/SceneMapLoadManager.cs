using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMapLoadManager : MonoBehaviourPun, IPunObservable
{
	public static SceneMapLoadManager Instance = null;

    public string loadMapName;

	public string curMapName = null;

    public Vector3 currentMapSpawnPoint;

    public Vector3 basecampSpawnPoint;

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

    private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoad;
		SceneManager.sceneUnloaded += OnSceneUnLoad;
	}

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		Debug.Log($"�ε�� �� : {scene.name} / �ε� ��� : {mode}");

		if(scene.name == "StartScene")
		{
			GameManager.Instance.GameStart();
			basecampSpawnPoint = GameObject.Find("OutDoorPoint").transform.position;
        }
	}


	// �� ����
	public void SetCurrentMap(string mapName, GameObject player)
	{
		var view = player.GetPhotonView();

		if(view == null) { return; }


		if (mapName == string.Empty)
		{
			print("�� ���� ���");
			photonView.RPC("MapNameSet", RpcTarget.AllBuffered, mapName);
		}
		else
		{
			if (DataManager.Instance.mapDataDic.ContainsKey(mapName) == false)
			{
				print("�� ���� ����");
				return;
			}
			else
			{
				print("�� ���� ����");
				photonView.RPC("MapNameSet", RpcTarget.AllBuffered, mapName);
			}
		}
    }

	[PunRPC]
	public void MapNameSet(string mapName)
	{
		loadMapName = mapName;
    }

	public void MakeMap()
	{
		print("�� ���� ����");
		var curmap = PhotonNetwork.Instantiate($"Map/{loadMapName}",
			GameObject.Find("MapSetPoint").transform.position, Quaternion.identity);

		if (curmap != null) { print($"{curmap.name}"); }

		curMapName = curmap.name;
		print($"�� ���� �Ϸ� : {curMapName}");

		photonView.RPC("SetCurrentMapSpawnPoint", RpcTarget.AllBuffered);

		var map = GameObject.Find($"{curMapName}");

        ItemSpawnManager.Instance.ItemSpawn(Random.Range(5, 10), map.transform.Find("ItemSpawnPoint"));
    }

	[PunRPC]
	private void SetCurrentMapSpawnPoint()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			var map = GameObject.Find($"{curMapName}");

			currentMapSpawnPoint = map.transform.Find("PlayerMovePoint").position;
		}
	}


	// �� ����
	public void PlayerDeleteMap(GameObject player)
	{
        PhotonView view = player.GetComponent<PhotonView>();

		if(view != null)
		{
			if (view.Owner.IsMasterClient)
			{
				DeleteMap();
			}
			else
			{
				photonView.RPC("DeleteMap", RpcTarget.MasterClient);
			}

			photonView.RPC("DeleteCurrentMapSpawnPoint", RpcTarget.AllBuffered);
		}
    }

	[PunRPC]
	private void DeleteMap()
	{
		var map = GameObject.Find($"{curMapName}");

		if(map != null)
		{
			PhotonNetwork.Destroy(map);
		}
	}

	[PunRPC]
	private void DeleteCurrentMapSpawnPoint()
	{
		currentMapSpawnPoint = basecampSpawnPoint;
	}


	private void OnSceneUnLoad(Scene scene)
	{
		Debug.Log($"����� �� : {scene.name}");

		if(scene.name == "StartScene")
		{
			
		}
	}

	private void OnDisable()
	{
		SceneManager.sceneUnloaded -= OnSceneUnLoad;
		SceneManager.sceneLoaded -= OnSceneLoad;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			stream.SendNext(loadMapName);
			stream.SendNext(curMapName);
			stream.SendNext(currentMapSpawnPoint);
		}
        else
        {
            loadMapName = (string)stream.ReceiveNext();
            curMapName = (string)stream.ReceiveNext();
            currentMapSpawnPoint = (Vector3)stream.ReceiveNext();
        }
    }
}
