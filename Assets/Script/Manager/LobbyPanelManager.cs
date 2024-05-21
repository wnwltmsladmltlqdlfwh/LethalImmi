using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyPanelManager : MonoBehaviourPunCallbacks
{
	public Dictionary<string, GameObject> panelDic;
	public LoginPanel loginPanel;
	public MainMenuPanel mainMenuPanel;

	public Dictionary<string, GameObject> mainMenuDic;
	public GameObject nullPanel;
	public CreateRoomMenu createRoomPanel;
	public JoinPanel joinRoomPanel;
	//public GameObject settingPanel;
	public GameObject quitgamePanel;

	private void Awake()
	{
		Transform panelParent = GameObject.Find("Canvas").transform.Find("PanelParent");
		loginPanel = panelParent.GetComponentInChildren<LoginPanel>();
		mainMenuPanel = panelParent.GetComponentInChildren<MainMenuPanel>();

		nullPanel = panelParent.Find("NullPanel").gameObject;
		createRoomPanel = panelParent.GetComponentInChildren<CreateRoomMenu>();
		joinRoomPanel = panelParent.GetComponentInChildren<JoinPanel>();
	}

	private void Start()
	{
		panelDic = new Dictionary<string, GameObject>
		{
			{ "Login", loginPanel.gameObject },
			{ "MainMenu", mainMenuPanel.gameObject }
		};

		mainMenuDic = new Dictionary<string, GameObject>
		{
			{ "None", nullPanel },
			{ "Create", createRoomPanel.gameObject },
			{ "Join", joinRoomPanel.gameObject },
		};

		
		ChangeLoginMainPanel("Login");
		UseInMainPanel("None");
	}

	public override void OnConnected()
	{
		Debug.Log("연결");

        ChangeLoginMainPanel("MainMenu");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
		Debug.Log("연결 해제 : " + cause);
		ChangeLoginMainPanel("Login");
	}

	public override void OnCreatedRoom()
	{
		base.OnCreatedRoom();
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		base.OnJoinRoomFailed(returnCode, message);
	}

	public override void OnLeftLobby()
	{
		base.OnLeftLobby();
		foreach (Transform roomInfo in joinRoomPanel.roomListRect)
		{
			Destroy(roomInfo.gameObject);
		}
		joinRoomPanel.isLeaveLobby = false;
		print("로비 떠나기");
	}
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		joinRoomPanel.RoomListUpdate(roomList);
		print("룸 리스트 업데이트" + PhotonNetwork.CountOfRooms);
	}

	public void ChangeLoginMainPanel(string panelname)
	{
		foreach(var panel in panelDic)
		{
			panel.Value.SetActive(panel.Key == panelname);
		}
	}

	public void UseInMainPanel(string menuName)
	{
		foreach(var menu in mainMenuDic)
		{
			menu.Value.SetActive(menu.Key == menuName);
		}
	}
}
