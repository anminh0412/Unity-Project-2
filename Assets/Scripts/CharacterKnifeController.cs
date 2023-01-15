using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterKnifeController : MonoBehaviourPunCallbacks
{
    public float damage;
    public GameObject author;
    public GameObject pos;
    Rigidbody rb;
    PhotonView view;
    //List<GameObject> targetList = new List<GameObject>();
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }
    private void FixedUpdate()
    {
        if(pos != null)
            rb.MovePosition(pos.transform.position);
    }
    public void FindAuthor(GameObject currentAuthor)
    {
        author = currentAuthor;
    }
    public void GetPos(GameObject posTemp)
    {
        pos = posTemp;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (view.IsMine) 
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject == author)
                {
                    return;
                }
                else
                {
                    collision.gameObject.GetComponent<PhotonView>().RPC("BulletSendDame", RpcTarget.All, damage, null);
                    if (collision.gameObject.GetComponent<CharacterHealthController>().currentHealth <= damage && collision.gameObject.GetComponent<CharacterHealthController>().immortal == false)
                    {
                        author.GetComponent<PhotonView>().RPC("GetKills", RpcTarget.All);
                    }
                }
            }
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<PhotonView>().RPC("EnemyTakeDame", RpcTarget.All, damage, true);
            }
            else
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            }
        }
    }

}
