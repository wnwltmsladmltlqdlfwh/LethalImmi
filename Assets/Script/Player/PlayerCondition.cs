using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerCondition : MonoBehaviour
{
	//체력
	public float health;
	public float maxHealth = 100;
	public float healthAmount;

	public bool isDead = false;

	// 스테미나
	public float stamina;
	private float maxStamina = 100;
	public float staminaAmount;

	private void Start()
	{
		ResetCondition();
	}

	private void Update()
	{
		if(this.GetComponent<PhotonView>().IsMine == false) { return; }

		staminaAmount = stamina / maxStamina;
		
		UIManager.Instance.staminaImage.fillAmount = staminaAmount;

		stamina = Mathf.Clamp(stamina, 0, maxStamina);
		health = Mathf.Clamp(health, 0, maxHealth);

		UIManager.Instance.healthImage.color = new Color(255f, 0f, 0f, healthAmount);
	}
	
	public void RunStaminaUpdate(bool value)
	{
		if(value)
		{
			stamina -= Time.deltaTime * 20f;
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
