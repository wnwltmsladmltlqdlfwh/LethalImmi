using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerInventory : MonoBehaviour
{
    public ItemData[] invenData = new ItemData[4];
    public int[] invenSellPrice = new int[4];
    public delegate void InvenDataChanged();
    public InvenDataChanged invenDataChanged;
    public int currentSlot = 0;

    [SerializeField]
    private Transform dropPoint;

    [SerializeField]
    private Transform handItemPos;
    private GameObject[] itemPrefabs = new GameObject[4];

	private void Start()
	{
        invenDataChanged += HandOnItem;
        invenDataChanged?.Invoke();
	}

	public void ActiveSlotChange(int value)
    {
        currentSlot = value - 1;
        invenDataChanged?.Invoke();
    }

	public void GetItemData(int curSellPrice, ItemData getItem)
    {
        for(int i = 0; i < invenData.Length; i++)
        {
            if (invenData[i] != null)
            {
                continue;
            }
            else if (invenData[i] == null)
            {
                invenData[i] = getItem;
                invenSellPrice[i] = curSellPrice;
                itemPrefabs[i] =
                    PhotonNetwork.Instantiate($"Item/{invenData[i].Item_PrefabName}", handItemPos.position, Quaternion.identity);
                if (itemPrefabs[i].GetComponent<Collider>() != null)
                {
                    itemPrefabs[i].GetComponent<Collider>().enabled = false;
                }
                if (itemPrefabs[i].GetComponent<Rigidbody>() != null)
                {
                    itemPrefabs[i].GetComponent<Rigidbody>().mass = 0f;
                    itemPrefabs[i].GetComponent<Rigidbody>().freezeRotation = true;
                    itemPrefabs[i].GetComponent<Rigidbody>().useGravity = false;
                }
                itemPrefabs[i].transform.parent = handItemPos.transform;
                itemPrefabs[i].transform.localPosition = Vector3.zero;
                itemPrefabs[i].transform.localRotation = Quaternion.identity;
				break;
            }
        }

        invenDataChanged?.Invoke();
    }

    public void DropItem()
    {
        if (invenData[currentSlot] == null) { return; }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        ItemSpawnManager.Instance.PlayerDropItem(invenSellPrice[currentSlot], invenData[currentSlot], ray.origin + ray.direction * 1f);
        invenData[currentSlot] = null;
        PhotonNetwork.Destroy(itemPrefabs[currentSlot]);
        if (itemPrefabs[currentSlot] != null) { itemPrefabs[currentSlot] = null; }

        invenDataChanged?.Invoke();
    }

    private void HandOnItem()
    {
        for(int i = 0; i < itemPrefabs.Length; i++)
        {
            if (itemPrefabs[i] == null)
            {
				return;
            }
            else
            {
                itemPrefabs[i].gameObject.SetActive(i == currentSlot);
			}
		}

        
	}

    public bool InvenIsFull()
    {
        foreach(ItemData item in invenData)
        {
            if(item == null)
            {
                return false;
            }
        }
        return true;
    }
}
