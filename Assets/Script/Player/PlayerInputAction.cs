using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAction : MonoBehaviour
{
	public Vector2 move;
	public Vector2 look;
	
	public bool jump;
	private bool isJumpPress = false;

	public bool sprint;

	public PlayerInteract playerInteract;
	public bool canMove;

	private void Awake()
	{
		playerInteract = GetComponent<PlayerInteract>();
		canMove = true;
	}

    private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputValue value)
	{
		if(canMove == false) { return; }
		move = value.Get<Vector2>();
	}

	public void OnLook(InputValue value) 
	{
		if (canMove == false || Cursor.lockState != CursorLockMode.Locked) { return; }
		look = value.Get<Vector2>();
	}

	public void OnJump(InputValue value)
	{
		if (canMove == false) { return; }

		if (this.GetComponent<PlayerCondition>().staminaAmount > 0.1f && isJumpPress == false)
		{
			jump = value.isPressed;
			//StartCoroutine(JumpPressBlock());
		}

		if(this.GetComponent<PlayerController>().isGrounded == true && jump == true)
		{
			this.GetComponent<PlayerCondition>().JumpStamina();
		}
	}

	private IEnumerator JumpPressBlock()
	{
		isJumpPress = true;
		yield return new WaitForSeconds(1f);
		isJumpPress = false;
	}

	public void OnSprint(InputValue value)
	{
		if (canMove == false) { return; }

		sprint = value.isPressed;
	}

	public void OnInteraction(InputValue value)
	{
		if (canMove == false) { return; }

		if (playerInteract.interactable != null)
		{
			if (value.isPressed == false)
			{
				UIManager.Instance.interactingCharger.fillAmount = 0f;
			}

			if (playerInteract.interactable.GetComponent<Interactable>().needLongPush == true)
            {
				gameObject.GetComponent<PlayerInteract>().isPressed = value.isPressed;
            }
			else
			{
				if (value.isPressed)
				{
					playerInteract.interactable.GetComponent<Interactable>().BaseInteract();
					playerInteract.interactable.GetComponent<Interactable>().BaseInteract(this.gameObject);
				}
			}
		}
		else
		{

		}
	}

	public void OnItemChange(InputValue value)
	{
		if (canMove == false) { return; }

		if (value.isPressed)
		{
			int a = int.Parse(value.Get().ToString());
			gameObject.GetComponent<PlayerInventory>().ActiveSlotChange(a);
		}
	}

	public void OnFire(InputValue value)
	{
		if (canMove == false) { return; }


	}

	public void OnDrop(InputValue value)
	{
		if (canMove == false) { return; }

		if (value.isPressed)
		{
			gameObject.GetComponent<PlayerInventory>().DropItem();
		}
	}

	public void OnRader(InputValue value)
	{
		if (canMove == false) { return; }

		if (value.isPressed)
		{
			gameObject.GetComponent<PlayerShotRader>().ShotRader();
		}
	}

	public void OnMenu(InputValue value)
	{
        if (canMove == false) { return; }

		if (value.isPressed)
		{
			gameObject.GetComponent<PlayerUI>().OnMenuPanel();
		}
    }
}
