using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasSetup : MonoBehaviour
{
	PhotonView phView;

	public GameObject myUIPrefab;
	public Image curStamina;
	public Image curHealth;
	public CanvasGroup damageOverlay;
	public TextMeshProUGUI promptText;
	public Image interactingCharger;
	public PlayerMenu menuCanvas;
	public Transform inventory;

	public void InitCanvas()
	{
		UIManager.Instance.staminaImage = curStamina;
		UIManager.Instance.healthImage = curHealth;
		UIManager.Instance.damageOverlay = damageOverlay;
		UIManager.Instance.promptText = promptText;
		UIManager.Instance.interactingCharger = interactingCharger;
		UIManager.Instance.p_Menu = menuCanvas;

		UIManager.Instance.p_Menu.gameObject.SetActive(false);

		foreach (var slot in inventory.GetComponentsInChildren<Image>())
		{
			if (slot.gameObject.name.Contains("Slot") == true)
			{
				UIManager.Instance.inventorySlots.Add(slot);
			}
		}

		UIManager.Instance.invenDataChanged?.Invoke();
	}
}
