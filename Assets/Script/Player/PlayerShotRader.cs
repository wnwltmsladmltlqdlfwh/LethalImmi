using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotRader : MonoBehaviour
{
    [SerializeField]
    GameObject rader;
	bool canShotRader = true;
	public float expansionRate;
	public float currentSize;
	public float maxSize;

	public void ShotRader()
	{
		if(canShotRader == false) { return; }
		GameObject raderPrefab = Instantiate(rader, transform.position, Quaternion.identity);
		StartCoroutine(ExpandSphere(raderPrefab));
	}

	IEnumerator ExpandSphere(GameObject sphere)
	{
		canShotRader = false;

		while (currentSize < maxSize)
		{
			currentSize += expansionRate * Time.deltaTime;
			sphere.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
			yield return null;
		}

		canShotRader = true;
		currentSize = 0f;

		Destroy(sphere);
	}
}
