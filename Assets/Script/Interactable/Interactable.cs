using Photon.Pun;
using UnityEngine;

public abstract class Interactable : MonoBehaviourPunCallbacks
{
    public bool useEvents;

    // 상호작용 가능한 오브젝트를 보고 있으면 노출되는 텍스트
    public string promptMessage;

    public bool needLongPush = false;

	private void Start()
	{
        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
	}

	public void BaseInteract()
    {
        if (useEvents) // Event를 사용하는 상호작용 오브젝트라면
        {
            GetComponent<InteractionEvent>().OnInteractEvent.Invoke();  // 추가한다.
        }
        Interact();
    }

    public void BaseInteract(GameObject player)
    {
        Interact(player);
    }

	protected virtual void Interact()
    {
        // 해당 클래스를 상속한 자식 클래스(각 오브젝트)에서 작성
    }
	protected virtual void Interact(GameObject player)  // 상호작용하는 플레이어의 정보 제공
	{
		// 해당 클래스를 상속한 자식 클래스(각 오브젝트)에서 작성
	}
}
