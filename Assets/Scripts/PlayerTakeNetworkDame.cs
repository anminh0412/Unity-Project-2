using UnityEngine;
using Photon.Pun;

public class PlayerTakeNetworkDame : MonoBehaviour
{
    public GameObject shakeCam;
    [PunRPC]
    void BulletSendDame(float _dame, GameObject _author)
    {
        GetComponent<CharacterHealthController>().CharacterTakeDame(_dame, false, _author);
    }
    [PunRPC]
    void GetKills()
    {
        GetComponent<CharacterHealthController>().GetKill(1);
    }
    [PunRPC]
     void CallShake()
    {
        shakeCam.GetComponent<Shake>().start = true;

    }
}
