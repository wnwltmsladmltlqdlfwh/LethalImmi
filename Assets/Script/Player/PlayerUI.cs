using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI promptText; // 시야에 닿은 상호작용 오브젝트에 대한 텍스트
    public Image interactingCharger; // 길게 눌러서 상호작용하는 오브젝트의 진행도 표시 UI

    public PlayerMenu p_Menu;

    public List<Image> inventorySlots;
	

	private void Start()
	{
        if(gameObject.GetComponent<PlayerInventory>() != null)
        {
            gameObject.GetComponent<PlayerInventory>().invenDataChanged += ChangedInventoryUI;
		}
	}

	public void UpdateText(string promptValue)
    {
        if (promptValue != string.Empty)
        {
            promptText.text = "[E] " + promptValue;
        }
        else
        {
            promptText.text = string.Empty;
        }
    }

	public void ChangedInventoryUI()
    {
		string[] isInvenData = gameObject.GetComponent<PlayerInventory>().invenKeyData;
        int activeSlot = gameObject.GetComponent<PlayerInventory>().currentSlot;

		for (int i = 0; i < inventorySlots.Count; i++)
        {
            var icon = inventorySlots[i].transform.Find("Icon").GetComponent<Image>();

            if (isInvenData[i] != string.Empty)
            {
                icon.enabled = true;
                icon.sprite = Resources.Load<Sprite>(DataManager.Instance.itemDataDic[isInvenData[i]].Item_Icon_Path);
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
            }

            inventorySlots[i].transform.Find("HighLight").gameObject.SetActive(i == activeSlot);
        }
    }

    public void OnMenuPanel()
    {
        if(p_Menu != null && p_Menu.gameObject.activeSelf == false)
        {
            p_Menu.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else if (p_Menu != null && p_Menu.gameObject.activeSelf == true)
        {
            p_Menu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
