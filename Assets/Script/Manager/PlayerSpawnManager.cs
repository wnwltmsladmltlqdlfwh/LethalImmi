using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawnManager : MonoBehaviourPunCallbacks
{
    public static PlayerSpawnManager Instance = null;

    public GameObject masterPlayer;
	public GameObject[] remotePlayer = new GameObject[3];

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
        base.OnEnable();    //PhotonNetwork.AddCallbackTarget(this); 메서드로 Callback 받을 대상 지정
    }

    public void MakePlayer(Transform spawnPos)
	{
		var player = PhotonNetwork.Instantiate("Player", spawnPos.position, Quaternion.identity);
		player.name = photonView.ViewID.ToString();
		print($"{player.name} 플레이어 생성 완료");
		
		GameObject setupCam = PhotonNetwork.Instantiate("CameraSetup", spawnPos.position, Quaternion.identity);
		string camName = $"{photonView.ViewID.ToString()}'s CameraSetup";
		setupCam.name = camName;
		print($"{camName} 카메라 생성 완료");

		
		GameObject uiCanvas = PhotonNetwork.Instantiate("UICanvas", spawnPos.position, Quaternion.identity);
		string canvasName = $"{photonView.ViewID.ToString()}'s Canvas";
		uiCanvas.name = canvasName;
		print($"{canvasName} 캔버스 생성 완료");
    }

	public void InitCanvas(GameObject canvas, GameObject player)
	{
		var uiAlive = GameObject.Find(canvas.name).transform.Find("StateAliveUIPanel");

		player.GetComponent<PlayerCondition>().staminaImage =
			uiAlive.transform.Find("StaminaBackGround").transform.Find("Stamina").GetComponent<Image>();
		player.GetComponent<PlayerCondition>().healthImage =
			uiAlive.transform.Find("HealthBackGround").transform.Find("CurrentHealth").GetComponent<Image>();
		player.GetComponent<Damageable>().damageOverlay =
			uiAlive.transform.Find("DamageOverlay").GetComponent<CanvasGroup>();
		player.GetComponent<PlayerUI>().promptText =
			uiAlive.transform.Find("PromptText").GetComponent<TextMeshProUGUI>();
		player.GetComponent<PlayerUI>().interactingCharger =
			uiAlive.transform.Find("InteractingChager").GetComponent<Image>();
		player.GetComponent<PlayerUI>().p_Menu =
			uiAlive.transform.Find("MenuCanvas").GetComponent<PlayerMenu>();

        player.GetComponent<PlayerUI>().p_Menu.gameObject.SetActive(false);

        foreach (var slot in uiAlive.transform.Find("Inventory").GetComponentsInChildren<Image>())
		{
			if (slot.gameObject.name.Contains("Slot") == true)
			{
				player.GetComponent<PlayerUI>().inventorySlots.Add(slot);
			}
		}

        player.GetComponent<PlayerInventory>().invenDataChanged?.Invoke();
    }

	public void InitCamera(GameObject camParent, GameObject player)
	{
		CinemachineVirtualCamera cam = camParent.transform.Find("1stCamera").GetComponent<CinemachineVirtualCamera>();

		if(player.GetComponent<PlayerInteract>() != null)
		{
			player.GetComponent<PlayerInteract>().cam = camParent.GetComponentInChildren<Camera>();
        }

		cam.Follow = player.transform.Find("CamFollow");
	}
}
