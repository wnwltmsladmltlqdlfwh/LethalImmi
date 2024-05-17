using Photon.Pun;
using UnityEngine;

public class PlayerInteract : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private float distance = 3f; // 카메라가 쏘는 ray의 길이
	[SerializeField]
	private LayerMask mask; // ray가 인식하는 Layer
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

		// 카메라의 위치에서 카메라 정면으로 쏘는 ray
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		// ray를 Scene에서 볼 수 있도록 그려줌
		Debug.DrawRay(ray.origin, ray.direction * distance);
		RaycastHit hitInfo; // ray에 닿은 오브젝트 정보

		if (Physics.Raycast(ray, out hitInfo, distance, mask))
		{
			interactable = hitInfo.collider.GetComponent<Interactable>();

			if (interactable != null)
			{
				playerUI.UpdateText(interactable.promptMessage);// playerUI에 있는 메서드로 Text 업데이트 해줌
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
