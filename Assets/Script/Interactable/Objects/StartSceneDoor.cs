using UnityEngine;

public class StartSceneDoor : Interactable
{
	private void Start()
	{
		needLongPush = true;
	}

	private void Update()
    {
        if (SceneMapLoadManager.Instance.loadMapName == string.Empty) { promptMessage = string.Empty; }
        else
        {
            promptMessage = $"{SceneMapLoadManager.Instance.loadMapName}으로 이동하기";
        }
    }

    protected override void Interact(GameObject player)
    {
        if(SceneMapLoadManager.Instance.currentMapSpawnPoint != Vector3.zero)
        {
            player.transform.position = SceneMapLoadManager.Instance.currentMapSpawnPoint;
        }
    }
}
