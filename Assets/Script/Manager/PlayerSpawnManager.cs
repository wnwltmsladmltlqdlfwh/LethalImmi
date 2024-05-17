using Photon.Pun;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviourPunCallbacks
{
    public static PlayerSpawnManager Instance = null;

    public GameObject localPlayer;

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
        base.OnEnable();    //PhotonNetwork.AddCallbackTarget(this); �޼���� Callback ���� ��� ����
    }

    public void MakePlayer(Transform spawnPos)
	{
		localPlayer = PhotonNetwork.Instantiate("Player", spawnPos.position, Quaternion.identity);
		localPlayer.name = photonView.ViewID.ToString();
		print($"{localPlayer.name} �÷��̾� ���� �Ϸ�");

		//GameObject setupCam = PhotonNetwork.Instantiate("CameraSetup", spawnPos.position, Quaternion.identity);
		//string camName = $"{photonView.ViewID.ToString()}'s CameraSetup";
		//setupCam.name = camName;
		//print($"{camName} ī�޶� ���� �Ϸ�");

		//GameObject uiCanvas = PhotonNetwork.Instantiate("UICanvas", spawnPos.position, Quaternion.identity);
		//string canvasName = $"{photonView.ViewID.ToString()}'s Canvas";
		//uiCanvas.name = canvasName;
		//print($"{canvasName} ĵ���� ���� �Ϸ�");
    }
}
