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
        base.OnEnable();    //PhotonNetwork.AddCallbackTarget(this); 메서드로 Callback 받을 대상 지정
    }

    public void MakePlayer(Transform spawnPos)
	{
		localPlayer = PhotonNetwork.Instantiate("Player", spawnPos.position, Quaternion.identity);
		localPlayer.name = photonView.ViewID.ToString();
		print($"{localPlayer.name} 플레이어 생성 완료");

		//GameObject setupCam = PhotonNetwork.Instantiate("CameraSetup", spawnPos.position, Quaternion.identity);
		//string camName = $"{photonView.ViewID.ToString()}'s CameraSetup";
		//setupCam.name = camName;
		//print($"{camName} 카메라 생성 완료");

		//GameObject uiCanvas = PhotonNetwork.Instantiate("UICanvas", spawnPos.position, Quaternion.identity);
		//string canvasName = $"{photonView.ViewID.ToString()}'s Canvas";
		//uiCanvas.name = canvasName;
		//print($"{canvasName} 캔버스 생성 완료");
    }
}
