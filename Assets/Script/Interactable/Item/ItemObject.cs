using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class ItemObject : Interactable, IPunObservable
{
    public ItemData itemData;

	public string item_name;
	public int sellPrice;
	public int buyPrice;
	public int weight;
	public string prefab_path;
	public string icon_Path;
	public string itemdata_Path;
	public string item_Key;

	public Sprite icon;
	GameObject itemInfoUI;


	private void Start()
	{
		this.gameObject.layer = LayerMask.NameToLayer("Interactable");
		
		itemInfoUI = (GameObject)Resources.Load("Item/ItemCanvas");

		if (item_name != null)
		{
			promptMessage = item_name;
		}
	}

	public void SetItemInfo(string itemKey, string itemDataPath)
	{
		this.itemData = (ItemData)Resources.Load(itemDataPath);

		if (itemData != null)
		{
			item_name = itemData.Item_Name;
			sellPrice = UnityEngine.Random.Range(itemData.Item_MinPrice, itemData.Item_MaxPrice);
			weight = itemData.Item_Weight;
			icon_Path = itemData.Item_Icon_Path;
            prefab_path = itemData.Item_PrefabPath;
			itemdata_Path = itemDataPath;
			item_Key = itemKey;
        }

		photonView.RPC("SyncItemInfo", RpcTarget.AllBuffered,
			itemDataPath, item_name, sellPrice, buyPrice, weight, icon_Path, prefab_path, item_Key);
	}

	[PunRPC]
	private void SyncItemInfo(string itemDataPath, string name, int sPrice, int bPrice, int w, string i_path, string pPath, string key)
	{
		itemData = (ItemData)Resources.Load(itemDataPath);
		item_name = name;
		sellPrice = sPrice;
		buyPrice = bPrice;
		weight = w;
        prefab_path = pPath;
		icon_Path = i_path;
		itemdata_Path = itemDataPath;
		item_Key = key;

		LoadIcon();
	}

    private void LoadIcon()
    {
        // 아이콘을 경로에서 로드
        icon = Resources.Load<Sprite>(icon_Path);

        if (icon == null)
        {
            Debug.LogWarning($"Failed to load icon at path: {icon_Path}");
        }
        else
        {
            // 아이콘이 로드된 이후의 처리가 필요하다면 이곳에 추가
            Debug.Log("Icon loaded successfully.");
        }
    }

    protected override void Interact(GameObject player)
    {
        var pView = player.GetPhotonView();

		print("Interact 실행");

        if (pView)
		{
			photonView.RPC("ItemTakeUser", RpcTarget.AllBuffered, pView.ViewID);
		}
	}

	[PunRPC]
	public void ItemTakeUser(int pView)
	{
        print("ItemTakeUser 실행");

        PhotonView photonView = PhotonView.Find(pView);
		if(photonView != null)
		{
			print("PhotonView ID 확인");

			GameObject player = photonView.gameObject;

			PlayerInventory pInven = player.GetComponent<PlayerInventory>();

			if (pInven != null)
			{
				if (pInven.InvenIsFull() == true)
				{
					print("인벤토리 꽉참 확인");
					return;
				}

                pInven.photonView.RPC("GetItemData", RpcTarget.AllBuffered, sellPrice, item_Key, this.photonView.ViewID);

				this.gameObject.SetActive(false);
			}
		}
	}

	public void ShowItemInfo()
	{
		GameObject canvasPrefab = Instantiate(itemInfoUI);

		canvasPrefab.transform.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.4f, transform.position.z);

		canvasPrefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item_name;
		canvasPrefab.transform.Find("SellPrice").GetComponent<TextMeshProUGUI>().text = sellPrice.ToString();
	}

	[PunRPC]
	public void SellThisItem()
	{
		GameManager.Instance.gameCount -= this.sellPrice;
		Destroy(gameObject);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			Debug.Log("데이터 발신");
			// 마스터 클라이언트에서 아이템의 데이터를 전송
			stream.SendNext(item_name);
			stream.SendNext(sellPrice);
			stream.SendNext(buyPrice);
			stream.SendNext(weight);
			stream.SendNext(prefab_path);
			stream.SendNext(icon_Path);
			stream.SendNext(item_Key);
			stream.SendNext(itemdata_Path);
		}
		else
		{
			try
			{
				// 리모트 플레이어에서 아이템의 데이터를 수신
				item_name = (string)stream.ReceiveNext();
				sellPrice = (int)stream.ReceiveNext();
				buyPrice = (int)stream.ReceiveNext();
				weight = (int)stream.ReceiveNext();
				prefab_path = (string)stream.ReceiveNext();
				icon_Path = (string)stream.ReceiveNext();
				item_Key = (string)stream.ReceiveNext();
				itemdata_Path = (string)stream.ReceiveNext();
			}
			catch(Exception e)
			{
				Debug.LogError("Error receiving data: " + e.Message);
			}
		}
	}
}
