using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NeworkExploed : MonoBehaviourPunCallbacks
{
    GameObject author;
    PhotonView view;
    //Damage
    public float explosionDamage;
    //public float explosionRange;
    //public float explosionForce;
    //List<GameObject> targetList = new List<GameObject>();
    private void Start()
    {
        //Explode();
        view = GetComponent<PhotonView>();
    }
    public void FindAuthor(GameObject Go)
    {
        author = Go;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!view.IsMine) return;
        if (other.CompareTag("Enemy")) 
        {
            other.GetComponent<PhotonView>().RPC("EnemyTakeDame", RpcTarget.All, explosionDamage, true);
        } 
        else if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PhotonView>().RPC("BulletSendDame", RpcTarget.All, explosionDamage, null);
            other.gameObject.GetComponent<PhotonView>().RPC("CallShake", RpcTarget.All);
            //other.gameObject.GetComponentInChildren<CameraShakeEffect>().ShakeOnce();
            if (other.gameObject.GetComponent<CharacterHealthController>().currentHealth <= explosionDamage && other.gameObject.GetComponent<CharacterHealthController>().immortal == false)
            {
                author.GetComponent<PhotonView>().RPC("GetKills", RpcTarget.All);
            }
        }
    }
    
    /*private void Explode()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRange);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].CompareTag("Enemy")) objects[i].GetComponent<TakeDameFromPlayer>().TakeDame(explosionDamage, true);
            else if (objects[i].CompareTag("Player")) objects[i].GetComponent<CharacterHealthController>().CharacterTakeDame(explosionDamage, false, author);
            //Add explosion force (if enemy has a rigidbody)
            if (objects[i].GetComponent<Rigidbody>())
                objects[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange, 3f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }*/
}
