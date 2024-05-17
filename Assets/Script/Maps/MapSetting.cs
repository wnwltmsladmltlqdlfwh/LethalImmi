using UnityEngine;
using Photon.Pun;

public class MapSetting : MonoBehaviourPun
{
	[SerializeField]
	Transform spawnPoint;

	private void Start()
	{
		print($"{gameObject.name} 생성");

		Vector3 setPos = spawnPoint.TransformPoint(spawnPoint.localPosition);
		photonView.RPC("SetSpawnPosition", RpcTarget.AllBuffered, spawnPoint.position);
		print("스폰 위치 전달");
	}

	[PunRPC]
	public void SetSpawnPosition(Vector3 pos)
	{
		SceneMapLoadManager.Instance.currentMapSpawnPoint = pos;
	}
}
