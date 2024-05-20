using Photon.Pun;
using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
	public static GameManager Instance = null;

    public bool gameStarted = false;

	PhotonView pView;

    public int gameCount = 100;

    public int overCount = 200;

    [SerializeField]
    GameObject gameOver;

    private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

    public override void OnEnable()
    {
        base.OnEnable();
    }

    private void Start()
    {
        pView = GetComponent<PhotonView>();
    }

    public void GameStart()
    {
        gameStarted = true;
        gameCount = 1;

        StartCoroutine(CheckWinCoroutine());
        StartCoroutine(CheckLoseCoroutine());
    }

    private IEnumerator CheckWinCoroutine()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            while (gameCount >= 0)
            {
                gameCount += Random.Range(1, 3);

                yield return new WaitForSeconds(1f);
            }
        }
        gameStarted = false;

		photonView.RPC("GameOver", RpcTarget.AllBuffered, true);
	}

	private IEnumerator CheckLoseCoroutine()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			while (gameCount <= overCount)
			{
				yield return new WaitForSeconds(1f);
			}
		}
		gameStarted = false;

		photonView.RPC("GameOver", RpcTarget.AllBuffered, false);
	}

	[PunRPC]
    public void GameOver(bool isWin)
    {
		gameCount = 100;

		StopCoroutine(CheckLoseCoroutine());
		StopCoroutine(CheckWinCoroutine());

		StartCoroutine(ShowGameOver(isWin));
    }

    IEnumerator ShowGameOver(bool isWin)
    {
		var parentCanvas = Instantiate(gameOver, FindObjectOfType<Canvas>().transform);

		var win = parentCanvas.transform.Find("WinOver");
		var lose = parentCanvas.transform.Find("LoseOver");

        CanvasGroup canvasGroup;

		if (isWin == true)
		{
            win.gameObject.SetActive(true);
            lose.gameObject.SetActive(false);
            canvasGroup = win.GetComponent<CanvasGroup>();
		}
		else
		{
			win.gameObject.SetActive(false);
			lose.gameObject.SetActive(true);
			canvasGroup = lose.GetComponent<CanvasGroup>();
		}

		canvasGroup.alpha = 0f;

		// 알파 값을 서서히 증가시키기
		while (canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += Time.deltaTime * 0.2f;
			yield return null; // 매 프레임마다 루프를 일시 중지
		}

		canvasGroup.alpha = 1f;

		yield return new WaitForSeconds(3f);

		PhotonNetwork.LeaveRoom();
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();

		Destroy(gameObject);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// 마스터 클라이언트인 경우에만 데이터를 전송합니다.
			stream.SendNext(gameStarted);
			stream.SendNext(gameCount);
			stream.SendNext(overCount);
		}
		else
		{
			// 마스터 클라이언트가 보낸 데이터를 수신합니다.
			gameStarted = (bool)stream.ReceiveNext();
			gameCount = (int)stream.ReceiveNext();
			overCount = (int)stream.ReceiveNext();
		}
	}
}
