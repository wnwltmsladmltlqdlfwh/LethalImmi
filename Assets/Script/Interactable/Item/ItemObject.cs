using Photon.Pun;
using TMPro;
using UnityEngine;

public class ItemObject : Interactable
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
		itemInfoUI = (GameObject)Resources.Load("Item/ItemCanvas");
		if (this.GetComponent<PhotonView>() != null)
		{
			phView = GetComponent<PhotonView>();
		}
		else
		{
			this.gameObject.AddComponent<PhotonView>();
		}

		this.gameObject.layer = LayerMask.NameToLayer("Interactable");

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
			PhotonNetwork.Destroy(gameObject);
		}
	}

    protected override void Interact(GameObject player)
    {
		if (player == PhotonNetwork.MasterClient.IsMasterClient)
		{
			ItemTakeUser(player);
		}
		else
		{
			photonView.RPC("ItemTakeUser", RpcTarget.All, player);
		}
	}

	public void ShowItemInfo()
	{
		GameObject canvasPrefab = Instantiate(itemInfoUI);

		canvasPrefab.transform.position = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.4f, transform.position.z);

		canvasPrefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item_name;
		canvasPrefab.transform.Find("SellPrice").GetComponent<TextMeshProUGUI>().text = sellPrice.ToString();
	}
}
