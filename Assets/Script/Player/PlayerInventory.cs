using Photon.Pun;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int[] invenSellPrice = new int[4];
    public string[] invenKeyData = new string[4];
    public delegate void InvenDataChanged();
    public InvenDataChanged invenDataChanged;
    public int currentSlot = 0;

    [SerializeField]
    private Transform dropPoint;

    public Transform handItemPos;
    public GameObject[] itemPrefabs = new GameObject[4];

    public PhotonView photonView;

	private void Start()
	{
        invenDataChanged += HandOnItem;
        invenDataChanged?.Invoke();
        photonView = GetComponent<PhotonView>();
	}

	public void ActiveSlotChange(int value)
    {
        currentSlot = value - 1;
        invenDataChanged?.Invoke();
    }

    [PunRPC]
	public void GetItemData(int curSellPrice, string getItemKey, int itemViewID)
    {
        print("GetItemData ½ÇÇà");

        for (int i = 0; i < invenKeyData.Length; i++)
        {
            if (invenKeyData[i] != string.Empty)
            {
                continue;
            }
            else if (invenKeyData[i] == string.Empty)
            {
                invenKeyData[i] = getItemKey;
                invenSellPrice[i] = curSellPrice;
                PhotonView itemPhotonView = PhotonView.Find(itemViewID);

                if(itemPhotonView != null)
                {
                    GameObject itemPrefab = itemPhotonView.gameObject;

                    itemPrefabs[i] = itemPrefab;
                
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
                }
                
				break;
            }
        }

        invenDataChanged?.Invoke();
    }


    [PunRPC]
    public void DropItem(Vector3 dropPos)
    {
        if (invenKeyData[currentSlot] == string.Empty) { return; }

        if (itemPrefabs[currentSlot] != null)
        {
            if (itemPrefabs[currentSlot].GetComponent<Collider>() != null)
            {
                itemPrefabs[currentSlot].GetComponent<Collider>().enabled = true;
            }
            if (itemPrefabs[currentSlot].GetComponent<Rigidbody>() != null)
            {
                itemPrefabs[currentSlot].GetComponent<Rigidbody>().freezeRotation = false;
                itemPrefabs[currentSlot].GetComponent<Rigidbody>().useGravity = true;
            }

            itemPrefabs[currentSlot].transform.SetParent(null);
            itemPrefabs[currentSlot].transform.localPosition = dropPos;
            itemPrefabs[currentSlot].transform.localRotation = Quaternion.identity;

            invenKeyData[currentSlot] = string.Empty;
            itemPrefabs[currentSlot] = null;
            invenSellPrice[currentSlot] = 0;


            invenDataChanged?.Invoke();
        }
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
        foreach(string item in invenKeyData)
        {
            if(item == string.Empty)
            {
                return false;
            }
        }
        return true;
    }
}
