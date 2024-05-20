using Photon.Pun;
using UnityEngine;

public class PlayerInteract : MonoBehaviourPunCallbacks
{
	public Camera cam;

	[SerializeField]
	private float distance = 3f; // ī�޶� ��� ray�� ����
	[SerializeField]
	private LayerMask mask; // ray�� �ν��ϴ� Layer
	private PlayerUI playerUI;
	PlayerInputAction playerInputAction;

	public Interactable interactable;

	public bool isPressed = false;
	float timer = 0f;
	public float maxTime;

	private void Start()
	{
		playerUI = GetComponent<PlayerUI>();
		playerInputAction = GetComponent<PlayerInputAction>();
	}

	private void Update()
	{
		if (this.GetComponent<PhotonView>().IsMine == false) { return; }

		playerUI.UpdateText(string.Empty);

		// ī�޶��� ��ġ���� ī�޶� �������� ��� ray
		Ray ray = new Ray(cam.transform.position, cam.transform.forward);
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
			gameObject.GetComponent<PlayerUI>().interactingCharger.fillAmount = 0f;
		}
		else
		{
			if (isPressed == true)
			{
				timer += Time.deltaTime;

				if (gameObject.GetComponent<PlayerUI>().interactingCharger.fillAmount >= 1f)
				{
					interactable.GetComponent<Interactable>().BaseInteract();
					interactable.GetComponent<Interactable>().BaseInteract(this.gameObject);
					timer = 0f;
					isPressed = false;
				}

				gameObject.GetComponent<PlayerUI>().interactingCharger.fillAmount = timer / maxTime;
			}
			else
			{
				timer = 0f;
				isPressed = false;
			}
		}

        RaycastHit hitMonster;

		if(Physics.Raycast(ray, out hitMonster))
		{
			var monster = hitMonster.collider.GetComponent<MageGhostAI>();


			if(monster != null)
			{
				monster.SetPlayerVisible(true);
            }
			else
			{
				return;
            }
        }
    }
}
