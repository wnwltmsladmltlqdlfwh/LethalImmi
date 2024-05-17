using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance = null;

	public CameraSetup cameraObject;

	public UICanvasSetup uiObject;

	public Image healthImage;
	public Image staminaImage;

	public TextMeshProUGUI promptText; // �þ߿� ���� ��ȣ�ۿ� ������Ʈ�� ���� �ؽ�Ʈ
	public Image interactingCharger; // ��� ������ ��ȣ�ۿ��ϴ� ������Ʈ�� ���൵ ǥ�� UI
	
	public PlayerMenu p_Menu;

	public delegate void InvenDataChanged();
	public InvenDataChanged invenDataChanged;
	public List<Image> inventorySlots;

	public CanvasGroup damageOverlay;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			if (Instance != this)
				Destroy(this.gameObject);
		}
	}

	private void Start()
	{
		uiObject.InitCanvas();

		uiObject.gameObject.SetActive(false);

		DontDestroyOnLoad(cameraObject.gameObject);
		DontDestroyOnLoad(uiObject.gameObject);
	}
}
