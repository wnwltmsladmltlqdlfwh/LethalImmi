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
		Debug.Log($"로드된 씬 : {scene.name} / 로드 모드 : {mode}");

		if(scene.name == "StartScene")
		{
			GameManager.Instance.GameStart();
			basecampSpawnPoint = GameObject.Find("OutDoorPoint").transform.position;
        }
	}

	public void SetCurrentMap(string mapName, GameObject player)
	{
		var view = player.GetPhotonView();

		if(view == null) { return; }


		if (mapName == string.Empty)
		{
			print("맵 세팅 취소");
			photonView.RPC("MapNameSet", RpcTarget.AllBuffered, mapName);
		}
		else
		{
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
    }

	[PunRPC]
	public void MapNameSet(string mapName)
	{
		loadMapName = mapName;
    }

    public void PlayerLoadMap(GameObject player)
	{
		PhotonView view = player.GetComponent<PhotonView>();

		if (view != null)
		{
			photonView.RPC("MakeMap", RpcTarget.MasterClient, loadMapName);
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
            photonView.RPC("DeleteMap", RpcTarget.MasterClient);
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

    private void OnSceneUnLoad(Scene scene)
	{
		Debug.Log($"종료된 씬 : {scene.name}");

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
			stream.SendNext(curMapName);
		}
        else
        {
			curMapName = (string)stream.ReceiveNext();
        }
    }
}
