using TMPro;
using UnityEngine;
using Photon.Pun;

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
        if(GameManager.Instance.gameStarted == false)
        {
            needMoneyTmp.text = "대기중..";
		}
        else
        {
            needMoneyTmp.text =
                $"현재 금액 : {GameManager.Instance.gameCount.ToString()}\n한도 : {GameManager.Instance.overCount.ToString()}";
        }
    }

    protected override void Interact(GameObject player)
    {
        base.Interact(player);

        var inven = player.GetComponent<PlayerInventory>();

        if (inven != null)
        {
            if (player.GetPhotonView().Owner.IsMasterClient)
            {
                for (int i = 0; i < inven.invenSellPrice.Length; i++)
                {
                    if(inven.invenSellPrice[i] == 0) { continue; }

					inven.itemPrefabs[i].SetActive(true);
					inven.itemPrefabs[i].GetComponent<ItemObject>().SellThisItem();

					inven.invenSellPrice[i] = 0;
                    inven.invenKeyData[i] = string.Empty;
                    inven.itemPrefabs[i] = null;
                    inven.invenDataChanged?.Invoke();
                }
            }
            else
            {
                for (int i = 0; i < inven.invenSellPrice.Length; i++)
                {
					if (inven.invenSellPrice[i] == 0) { continue; }

					inven.itemPrefabs[i].SetActive(true);
                    inven.itemPrefabs[i].GetComponent<ItemObject>().GetComponent<PhotonView>().RPC(
						"SellThisItem", RpcTarget.MasterClient);

					inven.invenSellPrice[i] = 0;
                    inven.invenKeyData[i] = string.Empty;
                    inven.itemPrefabs[i] = null;
					inven.invenDataChanged?.Invoke();
                }
            }
        }
    }

    [PunRPC]
    void GameCountDown(int sell)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.gameCount -= sell;
        }
    }
}
