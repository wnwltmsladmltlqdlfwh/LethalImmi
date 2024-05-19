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
            promptMessage = "�����ϱ�";
        }
        else
        {
            promptMessage = "������";
        }
    }

    protected override void Interact(GameObject player)
    {
        base.Interact(player);
        if (SceneMapLoadManager.Instance.currentMapSpawnPoint == Vector3.zero)
        {
            if (player.GetPhotonView().Owner == PhotonNetwork.MasterClient)
            {
                print("��ȣ�ۿ� �÷��̾�� ������");
                SceneMapLoadManager.Instance.MakeMap();
			}
            else
            {
                print("��ȣ�ۿ� �÷��̾�� ����Ʈ");
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
