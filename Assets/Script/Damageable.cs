using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
	private PlayerCondition isPlayer;

	// 플레이어
	public float damageTimeCount;
	public float damageTimeDelay;
	public Transform _cam;
	public float shakeTime = 0.5f;
	public float shakeSpeed = 3f;
	public float shakeAmount = 2f;
	public CanvasGroup damageOverlay;
	private bool overlayOff = false;


	private void Start()
	{
		isPlayer = GetComponent<PlayerCondition>();
		damageTimeDelay = 2f;
		_cam = transform.Find("CamFollow");
	}

	private void Update()
	{
		if (isPlayer != null)
		{
			if (GetComponent<PhotonView>().IsMine == false)
			{
				return;
			}
		}

        if (isPlayer.health <= 0f)
        {
            PhotonNetwork.LeaveRoom(true);
        }
    }

	[PunRPC]
	public void PlayerDamaged(float dmg)
	{
		damageTimeCount += Time.deltaTime;

		if (damageTimeCount < damageTimeDelay)
		{
			return;
		}

		print("플레이어 데미지 받음");

		isPlayer.health -= dmg;
		isPlayer.healthAmount = (isPlayer.maxHealth - isPlayer.health) / isPlayer.maxHealth;
		Debug.Log("데미지 : " + dmg);

		if (isPlayer.health <= 0f)
		{
			Destroy(gameObject);
			GameManager.Instance.GameOver(false);
		}

		damageTimeCount = 0f;
		StartCoroutine(ShakeCam());
		StartCoroutine(ShowDamageOverlay());

		if (damageOverlay.alpha != 0f && overlayOff == true)
		{
			damageOverlay.alpha -= Time.deltaTime * 2;
		}
		else if (damageOverlay.alpha == 0f)
		{
			overlayOff = false;
		}

		damageTimeCount = Mathf.Clamp(damageTimeCount, 0f, damageTimeDelay);
	}

	IEnumerator ShakeCam()
	{
		Vector3 originPos = _cam.localPosition;
		float elapsedTime = 0.0f;

		while (elapsedTime < shakeTime)
		{
			Vector3 randomPoint = originPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
			_cam.localPosition = Vector3.Lerp(_cam.localPosition, randomPoint, Time.deltaTime * shakeSpeed);

			yield return null;

			elapsedTime += Time.deltaTime;
		}

		_cam.localPosition = originPos;
	}

	IEnumerator ShowDamageOverlay()
	{
		damageOverlay.alpha = 1f;
		yield return new WaitForSeconds(1f);
		overlayOff = true;
	}
}
