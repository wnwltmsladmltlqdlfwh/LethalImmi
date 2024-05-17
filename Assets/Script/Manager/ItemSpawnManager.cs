using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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

	public void PlayerLoadItem(GameObject map, Transform itemSpawnPoint)
	{

		photonView.RPC("ItemSpawn", RpcTarget.MasterClient, Random.Range(5, 10));
	}

	[PunRPC]
	public void ItemSpawn(int makeCount, Transform itemSpawnPoint)
	{
		for (int i = 0; i < makeCount; i++)
		{
			int randomInt = Random.Range(0, DataManager.Instance.itemDataDic.Count);

			Vector3 spawnPos = itemSpawnPoint.position;
			Vector3 randomSetPos = spawnPos + Random.insideUnitSphere * 25f;
			Vector3 dontMakeUnderPos = new Vector3(randomSetPos.x, Random.Range(spawnPos.y, spawnPos.y + 10f), randomSetPos.z);

			GameObject itemPrefab = PhotonNetwork.Instantiate($"Item/{itemNameList[randomInt]}", dontMakeUnderPos, Quaternion.identity);

			if (itemPrefab.GetComponent<ItemObject>() == null)
			{
				itemPrefab.AddComponent<ItemObject>();
			}

			itemPrefab.GetComponent<ItemObject>().itemData =
					DataManager.Instance.itemDataDic[itemKeyList[randomInt]];

			photonView.RPC("InitializeItem", RpcTarget.AllBuffered, itemPrefab.GetPhotonView().ViewID);
		}
	}


	[PunRPC]
	public void InitializeItem(int viewID)
	{
		PhotonView itemView = PhotonView.Find(viewID);
		if (itemView != null)
		{
			ItemObject itemObject = itemView.GetComponent<ItemObject>();
			if (itemObject != null)
			{
				itemObject.SetItemInfo();
			}
		}
	}

	public void RequestDropItem(int curSellPrice, ItemData dropItemData, Vector3 dropPos)
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
			componentValue.icon_Path = dropItemData.Item_Icon_Path;
			componentValue.prefab_name = dropItemData.Item_PrefabName;

			componentValue.icon = Resources.Load<Sprite>(dropItemData.Item_Icon_Path);
		}
	}
}
