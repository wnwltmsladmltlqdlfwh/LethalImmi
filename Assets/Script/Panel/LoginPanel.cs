using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField idInputField;
	public TextMeshProUGUI nameInputMessage;
    public Button loginButton;
    public Button quitButton;

	private void OnEnable()
	{
		loginButton.onClick.AddListener(OnLoginButtonClick);
		quitButton.onClick.AddListener(OnQuitButtonClick);

		idInputField.interactable = true;
		loginButton.interactable = true;
		quitButton.interactable = true;
	}

	public void OnLoginButtonClick()
	{
		if(idInputField.text.ToCharArray().Length <= 2)
		{
			StartCoroutine(CharValueCoroutine());
			return;
		}
		
		PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues(idInputField.text);
		PhotonNetwork.NickName = idInputField.text;
		idInputField.interactable = false;
		loginButton.interactable = false;
		quitButton.interactable = false;

		if (PhotonNetwork.IsConnected)
		{
			Debug.Log("����� �����Դϴ�.");
			PhotonNetwork.GameVersion = "1.0";
		}
		else
		{
			Debug.Log("������ �õ��մϴ�.");
			PhotonNetwork.GameVersion = "1.0";
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	public void OnQuitButtonClick()
	{
		Application.Quit();
		Debug.Log("���� ����");
	}

	IEnumerator CharValueCoroutine()
	{
		nameInputMessage.text = "2���� �̻��� �̸��� �����ּ���.";
		nameInputMessage.gameObject.SetActive(true);
		yield return new WaitForSeconds(2f);
		nameInputMessage.gameObject.SetActive(false);
	}
}
