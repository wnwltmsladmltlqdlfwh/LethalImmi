using Photon.Pun;
using System.Collections;
using UnityEngine;

public class UICanvasSetup : MonoBehaviour
{
	PhotonView phView;
	public GameObject localPlayer;

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

		yield return null;
	}
}
