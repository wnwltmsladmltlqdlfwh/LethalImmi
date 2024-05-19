using UnityEngine;
using Photon.Pun;

public class MapSetting : MonoBehaviourPun
{
	[SerializeField]
	Transform playerMovePoint;

	public Transform itemSpawnPoint;

	private void OnEnable()
	{
		print($"{gameObject.name} ����");
	}

	private void OnDisable()
	{
		print($"{gameObject.name} ����");
	}
}
