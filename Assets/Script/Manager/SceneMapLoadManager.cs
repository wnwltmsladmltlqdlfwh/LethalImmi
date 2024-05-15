using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMapLoadManager : MonoBehaviourPunCallbacks
{
	public static SceneMapLoadManager Instance = null;

    public string loadMapName;

	public GameObject CurrentMap = null;

    public Transform currentMapSpawnPoint = null;

    public Transform basecampSpawnPoint = null;

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
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoad;
		SceneManager.sceneUnloaded += OnSceneUnLoad;
	}

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		Debug.Log($"로드된 씬 : {scene.name} / 로드 모드 : {mode}");

		if(scene.name == "StartScene")
		{
			GameManager.Instance.GameStart();
			basecampSpawnPoint = GameObject.Find("OutDoorPoint").transform;
        }
	}

	public void SetCurrentMap(string mapName, GameObject player)
	{
		var view = player.GetPhotonView();

		if(view == null) { return; }

		if (DataManager.Instance.mapDataDic.ContainsKey(mapName) == false)
		{
			print("맵 세팅 실패");
			return;
		}
		else
		{
			print("맵 세팅 성공");
			photonView.RPC("MapNameSet", RpcTarget.AllBuffered, mapName);
		}
    }

	[PunRPC]
	public void MapNameSet(string mapName)
	{
		loadMapName = mapName;
    }

    public IEnumerator PlayerLoadMap(GameObject player)
	{
		PhotonView view = player.GetComponent<PhotonView>();

		if (view != null && view.Owner.IsMasterClient == true)
		{
			MakeMap(loadMapName);
		}
		else if(view != null && view.Owner.IsMasterClient == false)
		{
			photonView.RPC("MakeMap", RpcTarget.MasterClient, loadMapName);
        }

        photonView.RPC("SetCurrentMap", RpcTarget.All);

        yield return null;
    }

	[PunRPC]
	private void MakeMap(string mapName)
	{
		if(mapName != null)
		{
			var curmap = PhotonNetwork.Instantiate($"Map/{loadMapName}",
				GameObject.Find("MapSetPoint").transform.position, Quaternion.identity);
        }
	}

	[PunRPC]
	private void SetCurrentMap()
	{
        print("문 찾기 시작");

        CurrentMap = GameObject.Find($"{loadMapName}(Clone)");
		currentMapSpawnPoint = CurrentMap.transform.Find("MapSpawnPoint");
		if (currentMapSpawnPoint == null)
		{
			print("문 찾기 실패");
		}
		else
		{
			print("문 찾기 성공");
		}

		if (currentMapSpawnPoint == null)
		{
			currentMapSpawnPoint = CurrentMap.transform.Find("StartPoint");
		}
	}

	public void PlayerDeleteMap(GameObject player)
	{
        PhotonView view = player.GetComponent<PhotonView>();

        if (view != null && view.Owner.IsMasterClient == true)
        {
            DeleteMap();
        }
        else if (view != null && view.Owner.IsMasterClient == false)
        {
            photonView.RPC("DeleteMap", RpcTarget.AllBuffered);
        }
    }

	[PunRPC]
	private void DeleteMap()
	{
		var map = GameObject.Find($"{loadMapName}");

		if(map != null)
		{
			PhotonNetwork.Destroy(map);
		}
	}

    private void OnSceneUnLoad(Scene scene)
	{
		Debug.Log($"종료된 씬 : {scene.name}");

		if(scene.name == "StartScene")
		{
			
		}
	}

	public override void OnDisable()
	{
		SceneManager.sceneUnloaded -= OnSceneUnLoad;
		SceneManager.sceneLoaded -= OnSceneLoad;
	}
}
