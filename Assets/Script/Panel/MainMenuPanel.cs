using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    LobbyPanelManager LobbyPanelManager;

    public Button hostButton;
    public Button joinButton;
    public Button settingButton;
    public Button quitButton;

	private void OnEnable()
	{
		LobbyPanelManager = FindObjectOfType<LobbyPanelManager>();
	}

	void Start()
    {
        hostButton.onClick.AddListener(OnHostButtonClick);
        joinButton.onClick.AddListener(OnJoinButtonClick);
        settingButton.onClick.AddListener(OnSettingButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
	}

	private void OnHostButtonClick()
    {
		LobbyPanelManager.UseInMainPanel("Create");
    }

    private void OnJoinButtonClick()
    {
        LobbyPanelManager.UseInMainPanel("Join");
	}

    private void OnSettingButtonClick()
    {

	}

    private void OnQuitButtonClick()
    {
        PhotonNetwork.Disconnect();
        LobbyPanelManager.ChangeLoginMainPanel("Login");
	}
}
