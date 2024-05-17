using Photon.Pun;
using TMPro;
using UnityEngine;

public class ItemObject : Interactable, IPunObservable
{
    public ItemData itemData;

	public string item_name;
	public int sellPrice;
	public int buyPrice;
	public int weight;
	public bool twoHanded;
	public Sprite icon;
	public string prefab_name;
	public string icon_Path;

	GameObject itemInfoUI;


	private void Start()
	{
		this.gameObject.layer = LayerMask.NameToLayer("Interactable");
		
		itemInfoUI = (GameObject)Resources.Load("Item/ItemCanvas");

		if(itemData != null)
		{
			photonView.RPC("SetItemInfo", RpcTarget.AllBuffered);
		}

		if (item_name != null)
		{
			promptMessage = item_name;
		}
	}

	public void SetItemInfo()
	{
		item_name = itemData.Item_Name;
		sellPrice = UnityEngine.Random.Range(itemData.Item_MinPrice, itemData.Item_MaxPrice);
		weight = itemData.Item_Weight;
		twoHanded = itemData.Item_TwoHanded;
		icon = itemData.Item_Icon;
		prefab_name = itemData.Item_PrefabName;

		photonView.RPC("SyncItemInfo", RpcTarget.AllBuffered, item_name, sellPrice, buyPrice, weight, twoHanded, itemData.Item_Icon_Path, prefab_name);
	}

	[PunRPC]
	private void SyncItemInfo(string name, int sPrice, int bPrice, int w, bool tHanded, string i_path, string pName)
	{
		item_name = name;
		sellPrice = sPrice;
		buyPrice = bPrice;
		weight = w;
		twoHanded = tHanded;
		prefab_name = pName;
		icon_Path = i_path;

		icon = Resources.Load<Sprite>(icon_Path); // 아이콘 로드
	}

	protected override void Interact(GameObject player)
    {
		if (player.GetPhotonView())
		{
			if (player.GetPhotonView().Owner.IsMasterClient)
			{
				ItemTakeUser(player);
			}
			else
			{
				photonView.RPC("ItemTakeUser", RpcTarget.MasterClient, player);
			}
		}
	}

	[PunRPC]
	public void ItemTakeUser(GameObject player)
	{
		if (player.GetComponent<PlayerInventory>() != null)
		{
			if (player.GetComponent<PlayerInventory>().InvenIsFull() == true)
			{ return; }

			player.GetComponent<PlayerInventory>().GetItemData(sellPrice, this.itemData);

			PhotonNetwork.Destroy(this.gameObject);
		}
	}

	public void ShowItemInfo()
	{
		GameObject canvasPrefab = Instantiate(itemInfoUI);

		canvasPrefab.transform.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.4f, transform.position.z);

		canvasPrefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item_name;
		canvasPrefab.transform.Find("SellPrice").GetComponent<TextMeshProUGUI>().text = sellPrice.ToString();
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// 마스터 클라이언트에서 아이템의 데이터를 전송
			stream.SendNext(item_name);
			stream.SendNext(sellPrice);
			stream.SendNext(buyPrice);
			stream.SendNext(weight);
			stream.SendNext(twoHanded);
			stream.SendNext(prefab_name);
			stream.SendNext(icon_Path);
		}
		else
		{
			// 리모트 플레이어에서 아이템의 데이터를 수신
			item_name = (string)stream.ReceiveNext();
			sellPrice = (int)stream.ReceiveNext();
			buyPrice = (int)stream.ReceiveNext();
			weight = (int)stream.ReceiveNext();
			twoHanded = (bool)stream.ReceiveNext();
			prefab_name = (string)stream.ReceiveNext();
			icon_Path = (string)stream.ReceiveNext();

			icon = Resources.Load<Sprite>(icon_Path);
		}
	}
}
