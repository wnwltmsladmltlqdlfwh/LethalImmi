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
            // 마스터 클라이언트인 경우에만 데이터를 전송합니다.
            stream.SendNext(gameStarted);
        }
        else
        {
            // 마스터 클라이언트가 보낸 데이터를 수신합니다.
            gameStarted = (bool)stream.ReceiveNext();
        }
    }
}
