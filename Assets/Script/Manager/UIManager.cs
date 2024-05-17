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

	public TextMeshProUGUI promptText; // 시야에 닿은 상호작용 오브젝트에 대한 텍스트
	public Image interactingCharger; // 길게 눌러서 상호작용하는 오브젝트의 진행도 표시 UI
	
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
