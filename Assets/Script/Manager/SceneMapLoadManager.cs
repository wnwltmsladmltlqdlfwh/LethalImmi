using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMapLoadManager : MonoBehaviourPunCallbacks
{
	public static SceneMapLoadManager Instance = null;

    public string loadMapName;

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
		loadMapName = "GameScene";
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
		}
	}

    public void PlayerLoadMap(GameObject player)
	{
		PhotonView view = player.GetComponent<PhotonView>();

		if (view != null && view.Owner.IsMasterClient)
		{
			PhotonSceneChange(loadMapName);
		}
		else
		{
			photonView.RPC("PhotonSceneChange", RpcTarget.All, loadMapName);
		}
	}

	[PunRPC]
	void PhotonSceneChange(string scene)
	{
		print(scene);
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
