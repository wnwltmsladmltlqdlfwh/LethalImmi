using Photon.Pun;
using System.Collections;
using UnityEngine;

public class UICanvasSetup : MonoBehaviour
{
	public PhotonView phView;
	public GameObject localPlayer;

    public RectTransform winCanvas;
    public RectTransform loseCanvas;

	private IEnumerator Start()
	{
		phView = GetComponent<PhotonView>();

        winCanvas.gameObject.SetActive(false);
		loseCanvas.gameObject.SetActive(false);

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetPhotonView().IsMine == true)
            {
                localPlayer = player;
            }
        }

        this.gameObject.SetActive(phView.IsMine);

		if (phView.IsMine)
		{
			PlayerSpawnManager.Instance.InitCanvas(this.gameObject, localPlayer);
		}

		localPlayer.GetComponent<PlayerInventory>().invenDataChanged?.Invoke();

        StartCoroutine(CountOverCoroutine());
        StartCoroutine(CheckWinCoroutine());

        yield return null;
    }

    private IEnumerator CheckWinCoroutine()
    {
        while (GameManager.Instance.gameCount > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(ShowGameOverUI(true));
    }

    private IEnumerator CountOverCoroutine()
    {
        while (GameManager.Instance.gameCount < 200)
        {
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(ShowGameOverUI(false));
    }

    public IEnumerator ShowGameOverUI(bool isWin)
    {
        if (isWin == true)
        {
            winCanvas.gameObject.SetActive(true);

            var canvasGroup = winCanvas.GetComponent<CanvasGroup>();

            canvasGroup.alpha = 0.0f;

            while (canvasGroup.alpha >= 1.0f)
            {
                canvasGroup.alpha += Time.deltaTime * 0.1f;
            }
        }
        else
        {
            loseCanvas.gameObject.SetActive(true);

            var canvasGroup = loseCanvas.GetComponent<CanvasGroup>();

            canvasGroup.alpha = 0.0f;

            while (canvasGroup.alpha >= 1.0f)
            {
                canvasGroup.alpha += Time.deltaTime * 0.1f;
            }
        }

        PhotonNetwork.LeaveRoom();

        yield return null;
    }
}
