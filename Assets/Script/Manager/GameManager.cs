using Photon.Pun;
using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
	public static GameManager Instance = null;

    public GameObject aliveUI;

    public bool gameStarted = false;

	PhotonView pView;

    public int gameCount = 100;

    private void Awake()
	{
        pView = GetComponent<PhotonView>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(this.gameObject);
        }
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
        gameCount = 100;

        StartCoroutine(CheckWinCoroutine());
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ Ŭ���̾�Ʈ�� ��쿡�� �����͸� �����մϴ�.
            stream.SendNext(gameStarted);
            stream.SendNext(gameCount);
        }
        else
        {
            // ������ Ŭ���̾�Ʈ�� ���� �����͸� �����մϴ�.
            gameStarted = (bool)stream.ReceiveNext();
            gameCount = (int)stream.ReceiveNext();
        }
    }

    private IEnumerator CheckWinCoroutine()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            while (gameCount > 0)
            {
                gameCount += Random.Range(3, 15);

                yield return new WaitForSeconds(10f);
            }
        }
        gameStarted = false;
    }
}
