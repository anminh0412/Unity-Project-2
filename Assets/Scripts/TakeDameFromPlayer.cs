using UnityEngine;
using Photon.Pun;

public class TakeDameFromPlayer : MonoBehaviour
{
    [PunRPC]
    public void EnemyTakeDame(float amount, bool isHeadShoot)
    {
        GetComponent<EnemyHealController>().HealthController(amount);
        GetComponent<EnemyHealController>().headShoot = isHeadShoot;
    }
}
