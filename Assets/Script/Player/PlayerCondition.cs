using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
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

		//if (damageOverlay.alpha != 0f && overlayOff == true)
		//{
		//	damageOverlay.alpha -= Time.deltaTime;
		//}
		//else if (damageOverlay.alpha == 0f)
		//{
		//	overlayOff = false;
		//}
	}
	
	//public void TakeDamage(float dmg)
	//{
	//	if(damageTimeCount < damageTimeDelay)
	//	{
	//		return;
	//	}

	//	health -= dmg;
	//	healthAmount = (maxHealth - health) / maxHealth;
	//	Debug.Log("데미지 : " + dmg);
	//	damageTimeCount = 0f;
	//	StartCoroutine(ShakeCam());
	//	StartCoroutine(ShowDamageOverlay());
	//}

	//IEnumerator ShakeCam()
	//{
	//	Vector3 originPos = _cam.localPosition;
	//	float elapsedTime = 0.0f;

	//	while(elapsedTime < shakeTime)
	//	{
	//		Vector3 randomPoint = originPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
	//		_cam.localPosition = Vector3.Lerp(_cam.localPosition, randomPoint, Time.deltaTime * shakeSpeed);

	//		yield return null;

	//		elapsedTime += Time.deltaTime;
	//	}

	//	_cam.localPosition = originPos;
	//}

	//IEnumerator ShowDamageOverlay()
	//{
	//	damageOverlay.alpha = 1f;
	//	yield return new WaitForSeconds(2f);
	//	overlayOff = true;
	//}

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
