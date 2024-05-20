using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviourPunCallbacks
{
	public static ItemSpawnManager Instance = null;

    public int spawnItems;

	public List<string> itemPathList = new List<string>();
	public List<string> itemKeyList = new List<string>();

    private void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

    private void Start()
	{
		foreach (var key in DataManager.Instance.itemDataDic.Keys)
		{
			string prefabPath = DataManager.Instance.itemDataDic[key].Item_PrefabPath;
			itemPathList.Add(prefabPath);
			itemKeyList.Add(key);
		}
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

			GameObject itemPrefab = PhotonNetwork.Instantiate($"{itemPathList[randomInt]}", dontMakeUnderPos, Quaternion.identity);

			if (itemPrefab.GetComponent<ItemObject>() == null)
			{
				itemPrefab.AddComponent<ItemObject>();
			}

			photonView.RPC("InitializeItem", RpcTarget.AllBuffered, itemPrefab.GetPhotonView().ViewID, itemKeyList[randomInt], DataManager.Instance.itemDataDic[itemKeyList[randomInt]].ItemData_Path);
		}
	}


	[PunRPC]
	public void InitializeItem(int viewID, string itemKey, string itemDataPath)
	{
		PhotonView itemView = PhotonView.Find(viewID);
		if (itemView != null)
		{
			ItemObject itemObject = itemView.GetComponent<ItemObject>();
			if (itemObject != null)
			{
				itemObject.SetItemInfo(itemKey, itemDataPath);
			}
		}
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();

		Destroy(gameObject);
	}
}
