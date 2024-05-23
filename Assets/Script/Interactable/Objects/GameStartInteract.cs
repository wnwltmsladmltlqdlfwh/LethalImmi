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
		    	textMeshPro.text = "����\n�������ּ���.";
                promptMessage = string.Empty;
		    }
            else
            {
		    	if (SceneMapLoadManager.Instance.curMapName == string.Empty)
		    	{
		    		textMeshPro.text = $"������ �������ּ���. \n ���� �� : {SceneMapLoadManager.Instance.loadMapName}";
                    promptMessage = "�����ϱ�";
		    	}
		    	else
		    	{
		    		textMeshPro.text = "������..";
		    		promptMessage = string.Empty;
		    	}
		    }
        }
        else
        {
			if (SceneMapLoadManager.Instance.curMapName == string.Empty)
			{
				textMeshPro.text = "�غ���..";
				promptMessage = "�����ϱ�";
			}
			else
			{
				textMeshPro.text = "������..";
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
                    print("��ȣ�ۿ� �÷��̾�� ������");
                    SceneMapLoadManager.Instance.MakeMap();
                    GameManager.Instance.GameStart();
                }
                else
                {
                    print("��ȣ�ۿ� �÷��̾�� ����Ʈ");
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
