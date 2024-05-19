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
	public string prefab_path;
	public string icon_Path;
	public string item_Key;

	public Sprite icon;
	GameObject itemInfoUI;


	private void Start()
	{
		this.gameObject.layer = LayerMask.NameToLayer("Interactable");
		
		itemInfoUI = (GameObject)Resources.Load("Item/ItemCanvas");

		//if(itemData != null)
		//{
		//	photonView.RPC("SetItemInfo", RpcTarget.AllBuffered);
		//}

		if (item_name != null)
		{
			promptMessage = item_name;
		}
	}

	public void SetItemInfo(string itemKey)
	{
        ItemData itemDataValue = this.itemData;

		if (itemData != null)
		{
			item_name = itemData.Item_Name;
			sellPrice = UnityEngine.Random.Range(itemData.Item_MinPrice, itemData.Item_MaxPrice);
			weight = itemData.Item_Weight;
			icon_Path = itemData.Item_Icon_Path;
            prefab_path = itemData.Item_PrefabPath;
			item_Key = itemKey;
        }

		photonView.RPC("SyncItemInfo", RpcTarget.AllBuffered,
			item_name, sellPrice, buyPrice, weight, icon_Path, prefab_path, item_Key);
	}

	[PunRPC]
	private void SyncItemInfo(string name, int sPrice, int bPrice, int w, string i_path, string pPath, string key)
	{
		item_name = name;
		sellPrice = sPrice;
		buyPrice = bPrice;
		weight = w;
        prefab_path = pPath;
		icon_Path = i_path;
		item_Key = key;

		LoadIcon();
	}
    private void LoadIcon()
    {
        // �������� ��ο��� �ε�
        icon = Resources.Load<Sprite>(icon_Path);

        if (icon == null)
        {
            Debug.LogWarning($"Failed to load icon at path: {icon_Path}");
        }
        else
        {
            // �������� �ε�� ������ ó���� �ʿ��ϴٸ� �̰��� �߰�
            Debug.Log("Icon loaded successfully.");
        }
    }

    protected override void Interact(GameObject player)
    {
        var pView = player.GetPhotonView();

		print("Interact ����");

        if (pView)
		{
			if (pView.Owner.IsMasterClient)
            {
                print("������ Ŭ�� Ȯ��");
                ItemTakeUser(pView.ViewID);
			}
			else
			{
                print("����Ʈ Ŭ�� Ȯ��");
                photonView.RPC("ItemTakeUser", RpcTarget.AllBuffered, pView.ViewID);
			}
		}
	}

	[PunRPC]
	public void ItemTakeUser(int pView)
	{
        print("ItemTakeUser ����");

        PhotonView photonView = PhotonView.Find(pView);
		if(photonView != null)
		{
			print("PhotonView ID Ȯ��");

			GameObject player = photonView.gameObject;

			PlayerInventory pInven = player.GetComponent<PlayerInventory>();

			if (pInven != null)
			{
				if (pInven.InvenIsFull() == true)
				{
					print("�κ��丮 ���� Ȯ��");
					return;
				}

                pInven.photonView.RPC("GetItemData", RpcTarget.AllBuffered, sellPrice, item_Key, this.photonView.ViewID);
                //pInven.GetItemData(sellPrice, this.itemData, this.photonView.ViewID);
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

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// ������ Ŭ���̾�Ʈ���� �������� �����͸� ����
			stream.SendNext(item_name);
			stream.SendNext(sellPrice);
			stream.SendNext(buyPrice);
			stream.SendNext(weight);
			stream.SendNext(prefab_path);
			stream.SendNext(icon_Path);
			stream.SendNext(item_Key);
		}
		else
		{
			// ����Ʈ �÷��̾�� �������� �����͸� ����
			item_name = (string)stream.ReceiveNext();
			sellPrice = (int)stream.ReceiveNext();
			buyPrice = (int)stream.ReceiveNext();
			weight = (int)stream.ReceiveNext();
			prefab_path = (string)stream.ReceiveNext();
			icon_Path = (string)stream.ReceiveNext();
			item_Key = (string)stream.ReceiveNext();
		}
	}
}
