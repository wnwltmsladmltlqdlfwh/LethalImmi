using Photon.Pun;
using UnityEngine;

public class GameStartInteract : Interactable
{
	private void Start()
	{
		needLongPush = true;
	}

	private void Update()
    {
        if(SceneMapLoadManager.Instance.curMapName == string.Empty)
        {
            promptMessage = "시작하기";
        }
        else
        {
            promptMessage = "끝내기";
        }
    }

    protected override void Interact(GameObject player)
    {
        base.Interact(player);
        if (SceneMapLoadManager.Instance.currentMapSpawnPoint == Vector3.zero)
        {
            if (player.GetPhotonView().Owner == PhotonNetwork.MasterClient)
            {
                print("상호작용 플레이어는 마스터");
                SceneMapLoadManager.Instance.MakeMap();
			}
            else
            {
                print("상호작용 플레이어는 리모트");
                photonView.RPC("RequestInteract", RpcTarget.MasterClient);
            }
		}
    }

    [PunRPC]
    void RequestInteract()
    {
        if(PhotonNetwork.IsMasterClient)
        {
			SceneMapLoadManager.Instance.MakeMap();
        }
    }
}
