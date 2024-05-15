using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBillborad : MonoBehaviour
{
    Transform maincam;

	private void Start()
	{
		maincam = Camera.main.transform;
		_ = StartCoroutine(CanvasFadeOut(gameObject));
	}

	IEnumerator CanvasFadeOut(GameObject prefab)
	{
		float time = 0f;
		while (time < 3f)
		{
			time += Time.deltaTime;
			yield return null;
		}
		Destroy(prefab);
	}

	void LateUpdate()
    {
        transform.LookAt(transform.position + maincam.rotation * Vector3.forward, maincam.rotation * Vector3.up);
    }
}
