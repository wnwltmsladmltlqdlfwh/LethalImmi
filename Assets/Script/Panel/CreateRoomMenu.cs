using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
	LobbyPanelManager LobbyPanelManager;

	public TMP_InputField roomName;
    public TMP_InputField maxPlayer;
    public Button createButton;
    public Button cancelButton;

	public override void OnEnable()
	{
		base.OnEnable();
		LobbyPanelManager = FindObjectOfType<LobbyPanelManager>();
		if (roomName.text != string.Empty) { roomName.text = string.Empty; }
		if (maxPlayer.text != "4") { maxPlayer.text = "4"; }
	}

	private void Start()
	{
		createButton.onClick.AddListener(CreateButtonClick);
		cancelButton.onClick.AddListener(CancelButtonClick);
	}

	public void CreateButtonClick()
	{
		int _maxPlayer;
		if (int.TryParse(maxPlayer.text, out _maxPlayer) == false)
		{
			Debug.Log("인원은 숫자만 입력해주십시오");
			return;
		}
		else if (int.Parse(maxPlayer.text) <= 0)
		{
			Debug.Log("1 이상의 숫자를 입력해주십시오");
			return;
		}
		else
			_maxPlayer = int.Parse(maxPlayer.text);

		string _roomName;
		if (string.IsNullOrEmpty(roomName.text))
			_roomName = PhotonNetwork.LocalPlayer.NickName + "의 크루";
		else
			_roomName = roomName.text;

		PhotonNetwork.CreateRoom(_roomName,
			new Photon.Realtime.RoomOptions()
			{
				MaxPlayers = _maxPlayer
			}
		);
	}

	public void CancelButtonClick()
	{
		LobbyPanelManager.UseInMainPanel("None");
	}
}
