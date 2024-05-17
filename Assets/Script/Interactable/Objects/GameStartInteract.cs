using Photon.Pun;
using System.Collections;
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
                SceneMapLoadManager.Instance.MakeMap();
			}
            else
            {
                PhotonView view = player.GetPhotonView();
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
