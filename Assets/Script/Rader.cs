using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rader : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<ItemObject>() != null)
		{
			other.GetComponent<ItemObject>().ShowItemInfo();
		}
	}
}
