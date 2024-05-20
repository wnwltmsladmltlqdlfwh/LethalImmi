using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCondition : MonoBehaviour
{
	PlayerInputAction inputAction;

	//체력
	public float health;
	public float maxHealth = 100;
	public float healthAmount;
	public Image healthImage;

	public bool isDead = false;

	// 스테미나
	public float stamina;
	private float maxStamina = 100;
	public float staminaAmount;
	public Image staminaImage;

	public Transform aliveUIPanel;

	private void Start()
	{
		ResetCondition();
	}

	private void Update()
	{
		if(this.GetComponent<PhotonView>().IsMine == false) { return; }

		staminaAmount = stamina / maxStamina;
		
		staminaImage.fillAmount = staminaAmount;

		stamina = Mathf.Clamp(stamina, 0, maxStamina);
		health = Mathf.Clamp(health, 0, maxHealth);

		healthImage.color = new Color(255f, 0f, 0f, healthAmount);
	}
	
	public void RunStaminaUpdate(bool value)
	{
		if(value)
		{
			stamina -= Time.deltaTime * 10f;
		}
		else
		{
			stamina += Time.deltaTime * 7f;
		}
	}

	public void JumpStamina()
	{
		stamina -= 10f;
	}

	public void ResetCondition()
	{
		health = maxHealth;

		stamina = maxStamina;
	}
}
