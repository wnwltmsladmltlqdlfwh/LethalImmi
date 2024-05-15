using Photon.Pun;
using UnityEngine;

public abstract class Interactable : MonoBehaviourPunCallbacks
{
    public bool useEvents;

    // ��ȣ�ۿ� ������ ������Ʈ�� ���� ������ ����Ǵ� �ؽ�Ʈ
    public string promptMessage;

    public bool needLongPush = false;

	private void Start()
	{
        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
	}

	public void BaseInteract()
    {
        if (useEvents) // Event�� ����ϴ� ��ȣ�ۿ� ������Ʈ���
        {
            GetComponent<InteractionEvent>().OnInteractEvent.Invoke();  // �߰��Ѵ�.
        }
        Interact();
    }

    public void BaseInteract(GameObject player)
    {
        Interact(player);
    }

	protected virtual void Interact()
    {
        // �ش� Ŭ������ ����� �ڽ� Ŭ����(�� ������Ʈ)���� �ۼ�
    }
	protected virtual void Interact(GameObject player)  // ��ȣ�ۿ��ϴ� �÷��̾��� ���� ����
	{
		// �ش� Ŭ������ ����� �ڽ� Ŭ����(�� ������Ʈ)���� �ۼ�
	}
}
