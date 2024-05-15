using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinPanel : MonoBehaviour
{
    public RectTransform roomListRect;
    
    List<RoomInfo> currentRoomList = new List<RoomInfo>();

	public bool isLeaveLobby = false;

    public GameObject roomPrefab;

	public Button refresh;

	private void Start()
	{
		refresh.onClick.AddListener(OnRefreshButtonClick);
	}

	void OnRefreshButtonClick()
	{
		_ = StartCoroutine(RefreshedRoomList());
	}

	private void OnEnable()
	{
		PhotonNetwork.JoinLobby();
	}

	public void RoomListUpdate(List<RoomInfo> roomList)
	{
		List<RoomInfo> destroyCandidate = currentRoomList.FindAll((x) => false == roomList.Contains(x));

		foreach (RoomInfo _room in roomList)
		{
			if (currentRoomList.Contains(_room))
			{
				continue;
			}
			AddRoomPrefab(_room);
		}

		foreach (Transform _room in roomListRect)
		{
			if (destroyCandidate.Exists((x) => x.Name == _room.name)) { Destroy(_room.gameObject); }
		}
	}

	private void AddRoomPrefab(RoomInfo info)
	{
		var room = Instantiate(roomPrefab, roomListRect, false);
		room.transform.Find("RoomName").GetComponent<TextMeshProUGUI>().text
			= info.Name;
		room.transform.Find("CurrentCrews").GetComponent<TextMeshProUGUI>().text
			= $"{info.PlayerCount} / {info.MaxPlayers}";
		room.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(()
			=> { PhotonNetwork.JoinRoom(info.Name); });
		print("规 府胶飘 积己"+info.Name);
	}

	IEnumerator RefreshedRoomList()
	{
		isLeaveLobby = true;
		PhotonNetwork.LeaveLobby();

		yield return new WaitWhile(() => isLeaveLobby);

		PhotonNetwork.JoinLobby();
	}

	private void OnDisable()
	{
		PhotonNetwork.LeaveLobby();
	}
}
