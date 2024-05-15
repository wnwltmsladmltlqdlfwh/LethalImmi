using Photon.Pun;
using UnityEngine;

public class InteractedDoor : Interactable
{
    [SerializeField]
    GameObject door;

    bool isOpen;


	// Start is called before the first frame update
	void Start()
    {
        door = transform.parent.gameObject;
	}

    // Update is called once per frame
    void Update()
    {
        switch (isOpen)
        {
            case false:
				needLongPush = true;
				promptMessage = "�� ����"; // ���� ���� ����
				break;
            case true:
				needLongPush = false;
				promptMessage = "�� �ݱ�";
                break;
        }
    }

    protected override void Interact()
    {
        photonView.RPC("OpenDoor", RpcTarget.All);
	}

    [PunRPC]
    void OpenDoor()
    {
		isOpen = !isOpen;
		door.GetComponent<Animator>().SetBool("isOpen", isOpen);
	}

}
