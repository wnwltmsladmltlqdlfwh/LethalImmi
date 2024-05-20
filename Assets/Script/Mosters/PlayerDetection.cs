using UnityEngine;
using Photon.Pun;

public class PlayerDetection : MonoBehaviour
{
    public MageGhostAI mageGhostAI;
    public Camera[] playerCameras;

    private void Start()
    {
        playerCameras = FindObjectsOfType<Camera>();
    }

    void Update()
    {
        if (mageGhostAI == null || playerCameras == null) return;

        foreach(Camera cam in playerCameras)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == mageGhostAI.transform)
                {
                    mageGhostAI.SetPlayerVisible(true);
                }
                else
                {
                    mageGhostAI.SetPlayerVisible(false);
                }
            }
            else
            {
                mageGhostAI.SetPlayerVisible(false);
            }
        }
    }
}