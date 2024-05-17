using UnityEngine;
using Photon.Pun;

public class MapSetting : MonoBehaviourPun
{
	[SerializeField]
	Transform spawnPoint;

	private void Start()
	{
		print($"{gameObject.name} ����");

		Vector3 setPos = spawnPoint.TransformPoint(spawnPoint.localPosition);
		photonView.RPC("SetSpawnPosition", RpcTarget.AllBuffered, spawnPoint.position);
		print("���� ��ġ ����");
	}

	[PunRPC]
	public void SetSpawnPosition(Vector3 pos)
	{
		SceneMapLoadManager.Instance.currentMapSpawnPoint = pos;
	}
}
