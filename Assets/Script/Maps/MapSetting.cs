using UnityEngine;
using Photon.Pun;

public class MapSetting : MonoBehaviourPun
{
	[SerializeField]
	Transform playerMovePoint;

	public Transform itemSpawnPoint;

	private void OnEnable()
	{
		print($"{gameObject.name} 생성");
	}

	private void OnDisable()
	{
		print($"{gameObject.name} 제거");
	}
}
