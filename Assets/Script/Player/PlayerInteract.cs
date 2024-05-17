using Photon.Pun;
using UnityEngine;

public class PlayerInteract : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private float distance = 3f; // ī�޶� ��� ray�� ����
	[SerializeField]
	private LayerMask mask; // ray�� �ν��ϴ� Layer
	private PlayerUI playerUI;

	public Interactable interactable;

	public bool isPressed = false;
	float timer = 0f;
	public float maxTime;

	private void Start()
	{
		playerUI = GetComponent<PlayerUI>();
	}

	private void Update()
	{
		if (this.GetComponent<PhotonView>().IsMine == false) { return; }

		playerUI.UpdateText(string.Empty);

		// ī�޶��� ��ġ���� ī�޶� �������� ��� ray
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		// ray�� Scene���� �� �� �ֵ��� �׷���
		Debug.DrawRay(ray.origin, ray.direction * distance);
		RaycastHit hitInfo; // ray�� ���� ������Ʈ ����

		if (Physics.Raycast(ray, out hitInfo, distance, mask))
		{
			interactable = hitInfo.collider.GetComponent<Interactable>();

			if (interactable != null)
			{
				playerUI.UpdateText(interactable.promptMessage);// playerUI�� �ִ� �޼���� Text ������Ʈ ����
			}
		}
		else
		{
			interactable = null;
		}
		//photonView.RPC("RayCasting", RpcTarget.All, ray);


		var interatable = gameObject.GetComponent<PlayerInputAction>().playerInteract.interactable;
		if (interatable == null)
		{
			timer = 0f;
			isPressed = false;
			UIManager.Instance.interactingCharger.fillAmount = 0f;
		}
		else
		{
			if (isPressed == true)
			{
				timer += Time.deltaTime;

				if (UIManager.Instance.interactingCharger.fillAmount >= 1f)
				{
					interactable.GetComponent<Interactable>().BaseInteract();
					interactable.GetComponent<Interactable>().BaseInteract(this.gameObject);
					timer = 0f;
					isPressed = false;
				}

				UIManager.Instance.interactingCharger.fillAmount = timer / maxTime;
			}
			else
			{
				timer = 0f;
				isPressed = false;
			}
		}
	}
}
