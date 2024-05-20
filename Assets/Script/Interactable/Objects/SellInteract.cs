using TMPro;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class SellInteract : Interactable
{
    public TextMeshPro needMoneyTmp;

    private void Start()
    {
        needLongPush = true;

        promptMessage = "판매하기";
    }

    private void Update()
    {
        needMoneyTmp.text = GameManager.Instance.gameCount.ToString();
    }

    protected override void Interact(GameObject player)
    {
        base.Interact(player);

        var inven = player.GetComponent<PlayerInventory>();

        if (inven != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < inven.invenSellPrice.Length; i++)
                {
                    SellingItem(inven.invenSellPrice[i]);
                    inven.invenSellPrice[i] = 0;
                    inven.invenKeyData[i] = string.Empty;
                    inven.itemPrefabs[i] = null;
                    Transform child = inven.handItemPos.GetChild(i);
                    child.gameObject.IsDestroyed();
                    inven.invenDataChanged?.Invoke();
                }
            }
            else
            {
                for (int i = 0; i < inven.invenSellPrice.Length; i++)
                {
                    photonView.RPC("SellingItem", RpcTarget.MasterClient, inven.invenSellPrice[i]);
                    inven.invenSellPrice[i] = 0;
                    inven.invenKeyData[i] = string.Empty;
                    inven.itemPrefabs[i] = null;
                    Transform child = inven.handItemPos.GetChild(i);
                    child.gameObject.IsDestroyed();
                    inven.invenDataChanged?.Invoke();
                }
            }
        }
    }

    [PunRPC]
    void SellingItem(int sell)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.gameCount -= sell;
        }
    }
}
