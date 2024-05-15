using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
	public static GameManager Instance = null;

    public GameObject aliveUI;

	public List<GameObject> managerList = new List<GameObject>();

    public bool gameStarted = false;

	PhotonView pView;

    private void Awake()
	{
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
    }

    public void GameEnd()
    {
        gameStarted = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ Ŭ���̾�Ʈ�� ��쿡�� �����͸� �����մϴ�.
            stream.SendNext(gameStarted);
        }
        else
        {
            // ������ Ŭ���̾�Ʈ�� ���� �����͸� �����մϴ�.
            gameStarted = (bool)stream.ReceiveNext();
        }
    }
}
