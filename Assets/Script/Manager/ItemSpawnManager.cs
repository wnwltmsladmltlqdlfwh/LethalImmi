using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviourPun
{
	public static ItemSpawnManager Instance = null;

    public int spawnItems;

	public List<string> itemNameList = new List<string>();
	public List<string> itemKeyList = new List<string>();

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
		foreach (var key in DataManager.Instance.itemDataDic.Keys)
		{
			string prefabName = DataManager.Instance.itemDataDic[key].Item_PrefabName;
			itemNameList.Add(prefabName);
			itemKeyList.Add(key);
		}
	}

	public void PlayerLoadItem(GameObject player)
	{
		PhotonView view = player.GetComponent<PhotonView>();

		if (view != null)
		{
			photonView.RPC("ItemSpawn", RpcTarget.MasterClient, 10);
		}
	}

	[PunRPC]
	public void ItemSpawn(int makeCount)
	{
		for (int i = 0; i < makeCount; i++)
		{
			int randomInt = Random.Range(0, DataManager.Instance.itemDataDic.Count);

			Vector3 spawnPos = GameObject.Find($"{SceneMapLoadManager.Instance.loadMapName}(Clone)").transform.position;
			Vector3 randomSetPos = spawnPos + Random.insideUnitSphere * 10f; 

			GameObject itemPrefab = PhotonNetwork.Instantiate($"Item/{itemNameList[randomInt]}", randomSetPos, Quaternion.identity);

			if (itemPrefab.GetComponent<ItemObject>() == null)
			{
				itemPrefab.AddComponent<ItemObject>();
			}

			itemPrefab.GetComponent<ItemObject>().itemData =
					DataManager.Instance.itemDataDic[itemKeyList[randomInt]];
		}
	}

	[PunRPC]
	private void SetItemInfo(ItemObject item, int itemKey)
	{
        item.itemData = DataManager.Instance.itemDataDic[itemKeyList[itemKey]];

        ItemObject componentValue = item.GetComponent<ItemObject>();
        ItemData itemDataValue = item.GetComponent<ItemObject>().itemData;

        if (componentValue.itemData != null)
        {
            componentValue.item_name = itemDataValue.Item_Name;
            componentValue.sellPrice = Random.Range(itemDataValue.Item_MinPrice, itemDataValue.Item_MaxPrice);
            componentValue.weight = itemDataValue.Item_Weight;
            componentValue.twoHanded = itemDataValue.Item_TwoHanded;
            componentValue.icon = itemDataValue.Item_Icon;
            componentValue.prefab_name = itemNameList[itemKey];
        }

        item.gameObject.name = componentValue.item_name;
    }

	public void RequestDestroyItem(int curSellPrice, ItemData dropItemData, Vector3 dropPos)
	{
		photonView.RPC("PlayerDropItem", RpcTarget.MasterClient, curSellPrice, dropItemData, dropPos);
	}

	[PunRPC]
	public void PlayerDropItem(int curSellPrice, ItemData dropItemData, Vector3 dropPos)
	{
		GameObject itemPrefab =
			PhotonNetwork.Instantiate($"Item/{dropItemData.Item_PrefabName}", dropPos, Quaternion.identity);

		if (itemPrefab.GetComponent<ItemObject>() == null)
		{
			itemPrefab.AddComponent<ItemObject>();
		}
		itemPrefab.GetComponent<ItemObject>().itemData = dropItemData;

		ItemObject componentValue = itemPrefab.GetComponent<ItemObject>();

		if (componentValue.itemData != null)
		{
			componentValue.item_name = dropItemData.Item_Name;
			componentValue.sellPrice = curSellPrice;
			componentValue.weight = dropItemData.Item_Weight;
			componentValue.twoHanded = dropItemData.Item_TwoHanded;
			componentValue.icon = dropItemData.Item_Icon;
			componentValue.prefab_name = dropItemData.Item_PrefabName;
		}
		itemPrefab.gameObject.name = componentValue.item_name;
	}
}
