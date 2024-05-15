using Photon.Pun;
using System.Collections;
using UnityEngine;

public class GameStartInteract : Interactable
{
    private void Update()
    {
        if(SceneMapLoadManager.Instance.CurrentMap == null)
        {
            promptMessage = "시작하기";
        }
        else
        {
            promptMessage = "끝내기";
        }
    }

    protected override void Interact(GameObject player)
    {
        base.Interact(player);
        if (SceneMapLoadManager.Instance.CurrentMap == null)
        {
            _= StartCoroutine(SceneMapLoadManager.Instance.PlayerLoadMap(player));
        }
        else
        {
            SceneMapLoadManager.Instance.PlayerDeleteMap(player);
        }
    }

    IEnumerator MakeMapCorountine(GameObject player)
    {
        SetTimeScale(0f);

        yield return new WaitUntil(() => IsMapGenerated());

        SetTimeScale(1f);
    }

    bool IsMapGenerated()
    {
        if(SceneMapLoadManager.Instance.CurrentMap == null)
            return false;
        else
            return true;
    }

    [PunRPC]
    void SetTimeScale(float time)
    {
        Time.timeScale = time;
    }
}
