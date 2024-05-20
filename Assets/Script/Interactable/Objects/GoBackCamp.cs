using UnityEngine;

public class GoBackCamp : Interactable
{
    private void Start()
    {
        needLongPush = true;
        promptMessage = "돌아가기";
    }


    protected override void Interact(GameObject player)
    {
        base.Interact(player);

        if(player != null)
        {
            player.transform.position = SceneMapLoadManager.Instance.basecampSpawnPoint;
        }
    }
}
