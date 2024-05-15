using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testVehicle : MonoBehaviour
{
    Dictionary<string, GameObject> vehicleDic = new Dictionary<string, GameObject>();

    void Start()
    {
		DontDestroyOnLoad(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Items")
        {
            other.gameObject.transform.parent = transform;
        }
	}

	private void OnTriggerExit(Collider other)
	{
		other.transform.parent = null;
	}
}
