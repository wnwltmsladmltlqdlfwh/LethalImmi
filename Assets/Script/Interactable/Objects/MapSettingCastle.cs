using UnityEngine;

public class MapSettingCastle : Interactable
{
    private void Start()
    {
        needLongPush = true;
    }

    private void Update()
    {
        if(SceneMapLoadManager.Instance.loadMapName == string.Empty)
        {
            promptMessage = "Castle";
        }
        else
        {
            promptMessage = "취소하기";
        }
    }

    protected override void Interact(GameObject player)
    {
        base.Interact(player);
        if(SceneMapLoadManager.Instance.loadMapName == string.Empty)
        {
            SceneMapLoadManager.Instance.SetCurrentMap(promptMessage, player);
        }
        else if(SceneMapLoadManager.Instance.loadMapName != null)
        {
            SceneMapLoadManager.Instance.SetCurrentMap(string.Empty, player);
        }
    }
}
