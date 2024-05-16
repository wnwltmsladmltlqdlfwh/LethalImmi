using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Unity.VisualScripting.Dependencies.NCalc;

public class testCom : Interactable
{
	[SerializeField]
	bool comIsOn = false;

	PlayerInput input;

	private void Start()
	{
		promptMessage = "ÄÄÇ»ÅÍ";
		input = GetComponent<PlayerInput>();
	}

	protected override void Interact(GameObject player)
	{
		if(comIsOn) { return; }

		UseComputer(player);
	}

	[PunRPC]
	void UseComputer(GameObject user)
	{
		print("ComTurnOn");

		comIsOn = true;

		user.GetComponent<PlayerInputAction>().canMove = false;
	}

	void OnTurnOff(InputValue value)
	{
		if(comIsOn == false) { return; }

		var inputSwitch = FindObjectsOfType<PlayerInputAction>();

		if (value.isPressed)
		{
			comIsOn = false;
			foreach (PlayerInputAction action in inputSwitch)
			{
				if (action.canMove == false)
				{
					action.canMove = true;
				}
			}
		}
	}
}
