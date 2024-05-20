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
        if(SceneMapLoadManager.Instance.mapIsOn == false)
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
            if (SceneMapLoadManager.Instance.mapIsOn == false)
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
            else
            {
                if (player.GetPhotonView().Owner == PhotonNetwork.MasterClient)
                {
                    print("상호작용 플레이어는 마스터");
                    SceneMapLoadManager.Instance.DeleteMap();
                }
                else
                {
                    print("상호작용 플레이어는 리모트");
                    photonView.RPC("RequestInteract", RpcTarget.MasterClient);
                }
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
            else
            {
                SceneMapLoadManager.Instance.DeleteMap();
            }
        }
    }
}
