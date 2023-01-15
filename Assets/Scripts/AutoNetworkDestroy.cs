using UnityEngine;
using Photon.Pun;

public class AutoNetworkDestroy : MonoBehaviour
{
    PhotonView view;
    [SerializeField] float destroyTime;
    void Start()
    {
        view = GetComponent<PhotonView>();
        Invoke("AutoDestroy", destroyTime);
    }
    void AutoDestroy()
    {
        if(view.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
