using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	private void Start()
	{
        if(gameObject.GetComponent<PlayerInventory>() != null)
        {
			UIManager.Instance.invenDataChanged += ChangedInventoryUI;
		}
	}

	public void UpdateText(string promptValue)
    {
        if (promptValue != string.Empty)
        {
            UIManager.Instance.promptText.text = "[E] " + promptValue;
        }
        else
        {
            UIManager.Instance.promptText.text = string.Empty;
        }
    }

	public void ChangedInventoryUI()
    {
		ItemData[] isInvenData = gameObject.GetComponent<PlayerInventory>().invenData;
        int activeSlot = gameObject.GetComponent<PlayerInventory>().currentSlot;

		for (int i = 0; i < UIManager.Instance.inventorySlots.Count; i++)
        {
            var icon = UIManager.Instance.inventorySlots[i].transform.Find("Icon").GetComponent<Image>();

            if (isInvenData[i] != null)
            {
                icon.enabled = true;
                icon.sprite = isInvenData[i].Item_Icon;
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
            }

            UIManager.Instance.inventorySlots[i].transform.Find("HighLight").gameObject.SetActive(i == activeSlot);
        }
    }

    public void OnMenuPanel()
    {
        if(UIManager.Instance.p_Menu != null && UIManager.Instance.p_Menu.gameObject.activeSelf == false)
        {
            UIManager.Instance.p_Menu.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else if (UIManager.Instance.p_Menu != null && UIManager.Instance.p_Menu.gameObject.activeSelf == true)
        {
            UIManager.Instance.p_Menu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
