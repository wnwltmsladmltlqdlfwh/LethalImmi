using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasSetup : MonoBehaviour
{
	public PhotonView phView;
	public GameObject localPlayer;

	[SerializeField]
	private Button isExitButton;

	private IEnumerator Start()
	{
		phView = GetComponent<PhotonView>();

		foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (player.GetPhotonView().IsMine == true)
			{
				localPlayer = player;
			}
		}

		this.gameObject.SetActive(phView.IsMine);

		if (phView.IsMine)
		{
			PlayerSpawnManager.Instance.InitCanvas(this.gameObject, localPlayer);
		}

		localPlayer.GetComponent<PlayerInventory>().invenDataChanged?.Invoke();

		isExitButton.onClick.AddListener(OnExitRoomButton);

		yield return null;
	}

	void OnExitRoomButton()
	{
		PhotonNetwork.LeaveRoom();
	}
}
