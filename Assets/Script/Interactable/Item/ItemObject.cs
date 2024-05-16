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

	GameObject itemInfoUI;
	PhotonView phView;


	private void Start()
	{
		this.gameObject.layer = LayerMask.NameToLayer("Interactable");
		
		itemInfoUI = (GameObject)Resources.Load("Item/ItemCanvas");
		if (this.GetComponent<PhotonView>() != null)
		{
			phView = GetComponent<PhotonView>();
		}
		else
		{
			this.gameObject.AddComponent<PhotonView>();
		}

		if(itemData != null)
		{
			photonView.RPC("SetItemInfo", RpcTarget.AllBuffered);
		}
		else
		{
			print("아이템 데이터가 없습니다.");
		}

		if (item_name != null)
		{
			promptMessage = item_name;
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

	public void ShowItemInfo()
	{
		GameObject canvasPrefab = Instantiate(itemInfoUI);

		canvasPrefab.transform.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.4f, transform.position.z);

		canvasPrefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item_name;
		canvasPrefab.transform.Find("SellPrice").GetComponent<TextMeshProUGUI>().text = sellPrice.ToString();
	}

	[PunRPC]
	private void SetItemInfo()
	{
		ItemData itemDataValue = this.itemData;

		if (itemData != null)
		{
			item_name = itemDataValue.Item_Name;
			sellPrice = UnityEngine.Random.Range(itemDataValue.Item_MinPrice, itemDataValue.Item_MaxPrice);
			weight = itemDataValue.Item_Weight;
			twoHanded = itemDataValue.Item_TwoHanded;
			icon = itemDataValue.Item_Icon;
			prefab_name = itemDataValue.Item_PrefabName;
		}

		gameObject.name = item_name;
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
			stream.SendNext(icon);
			stream.SendNext(prefab_name);
		}
		else
		{
			// 리모트 플레이어에서 아이템의 데이터를 수신
			item_name = (string)stream.ReceiveNext();
			sellPrice = (int)stream.ReceiveNext();
			buyPrice = (int)stream.ReceiveNext();
			weight = (int)stream.ReceiveNext();
			twoHanded = (bool)stream.ReceiveNext();
			icon = (Sprite)stream.ReceiveNext();
			prefab_name = (string)stream.ReceiveNext();
		}
	}
}
