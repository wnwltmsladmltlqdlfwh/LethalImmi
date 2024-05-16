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
        if (SceneMapLoadManager.Instance.curMapName == string.Empty)
        {
            if(player.GetPhotonView().Owner.IsMasterClient)
            {
                MakeMap(SceneMapLoadManager.Instance.loadMapName);
            }
            else
            {
			    photonView.RPC("MakeMap", RpcTarget.MasterClient, SceneMapLoadManager.Instance.loadMapName);
            }

			ItemSpawnManager.Instance.PlayerLoadItem(player);
		}
        else
        {
            SceneMapLoadManager.Instance.PlayerDeleteMap(player);
        }
    }

	[PunRPC]
	private void MakeMap(string mapName)
	{
		print("�� ���� ����");
		var curmap = PhotonNetwork.Instantiate($"Map/{mapName}",
			GameObject.Find("MapSetPoint").transform.position, Quaternion.identity);

		if (curmap != null) { print($"{curmap.name}"); }

		SceneMapLoadManager.Instance.curMapName = curmap.name;
		print($"�� ���� �Ϸ� : {SceneMapLoadManager.Instance.curMapName}");
	}

	[PunRPC]
    void SetTimeScale(float time)
    {
        Time.timeScale = time;
    }
}
