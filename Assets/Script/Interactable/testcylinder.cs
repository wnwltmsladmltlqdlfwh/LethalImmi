using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcylinder : Interactable
{
	public string test;

	private void Awake()
	{
		needLongPush = true;
	}

	protected override void Interact(GameObject player)
	{
		SceneMapLoadManager.Instance.PlayerLoadMap(player);
	}
}
