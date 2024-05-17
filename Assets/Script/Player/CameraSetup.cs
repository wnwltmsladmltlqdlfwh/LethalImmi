using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
	PhotonView phView;

	private void OnEnable()
	{
		StartCoroutine(SettingStart());
	}

	private IEnumerator SettingStart()
	{
		yield return new WaitWhile(() => PlayerSpawnManager.Instance.localPlayer == null);

		print($"{gameObject.name}은 {PlayerSpawnManager.Instance.localPlayer}를 찾았다.");

		phView = GetComponent<PhotonView>();

		this.gameObject.SetActive(phView.IsMine);

		if (phView.IsMine)
		{
			InitCamera(this.gameObject, PlayerSpawnManager.Instance.localPlayer);
		}

		yield return null;
	}

	public void InitCamera(GameObject camParent, GameObject player)
	{
		CinemachineVirtualCamera cam = camParent.transform.Find("1stCamera").GetComponent<CinemachineVirtualCamera>();

		cam.Follow = player.transform.Find("CamFollow");
	}
}
