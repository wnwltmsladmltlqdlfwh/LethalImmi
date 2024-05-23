using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameStartInteract : Interactable
{
    [SerializeField]
    TextMeshPro textMeshPro;

	private void Start()
	{
		needLongPush = true;
	}

	private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(SceneMapLoadManager.Instance.loadMapName == string.Empty)
            {
		    	textMeshPro.text = "맵을\n세팅해주세요.";
                promptMessage = string.Empty;
		    }
            else
            {
		    	if (SceneMapLoadManager.Instance.curMapName == string.Empty)
		    	{
		    		textMeshPro.text = $"방장이 시작해주세요. \n 현재 맵 : {SceneMapLoadManager.Instance.loadMapName}";
                    promptMessage = "시작하기";
		    	}
		    	else
		    	{
		    		textMeshPro.text = "진행중..";
		    		promptMessage = string.Empty;
		    	}
		    }
        }
        else
        {
			if (SceneMapLoadManager.Instance.curMapName == string.Empty)
			{
				textMeshPro.text = "준비중..";
				promptMessage = "시작하기";
			}
			else
			{
				textMeshPro.text = "진행중..";
				promptMessage = string.Empty;
			}

			promptMessage = string.Empty;
		}
    }

    protected override void Interact(GameObject player)
    {
        base.Interact(player);
        if (SceneMapLoadManager.Instance.currentMapSpawnPoint == Vector3.zero)
        {
            if (SceneMapLoadManager.Instance.mapIsOn == false)
            {
                if (player.GetPhotonView().Owner == PhotonNetwork.MasterClient)
                {
                    print("상호작용 플레이어는 마스터");
                    SceneMapLoadManager.Instance.MakeMap();
                    GameManager.Instance.GameStart();
                }
                else
                {
                    print("상호작용 플레이어는 리모트");
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    [PunRPC]
    void RequestInteract()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(SceneMapLoadManager.Instance.mapIsOn == false)
            {
			    SceneMapLoadManager.Instance.MakeMap();
            }
        }
    }
}
